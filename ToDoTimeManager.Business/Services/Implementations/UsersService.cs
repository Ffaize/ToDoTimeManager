using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Business.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IUsersDataController _usersDataController;
    private readonly IUserSecretsDataController _userSecretsDataController;
    private readonly IActivityLogsService _activityLogsService;
    private readonly ILogger<UsersService> _logger;
    private readonly IPasswordHelperService _passwordHelperService;

    public UsersService(
        IUsersDataController usersDataController,
        IUserSecretsDataController userSecretsDataController,
        IActivityLogsService activityLogsService,
        ILogger<UsersService> logger,
        IPasswordHelperService passwordHelperService)
    {
        _usersDataController       = usersDataController;
        _userSecretsDataController = userSecretsDataController;
        _activityLogsService       = activityLogsService;
        _logger                    = logger;
        _passwordHelperService     = passwordHelperService;
    }

    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            List<UserEntity> res = await _usersDataController.GetAllUsers();
            return res.Select(ue => ue.ToUser()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<User?> GetUserById(Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            var res = await _usersDataController.GetUserById(userId);
            if (res == null)
                throw new NotFoundException("User was not found");

            if (currentUserRole < UserRole.Manager && userId != currentUserId)
                throw new ForbiddenException();

            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByUsername(string username, Guid currentUserId, UserRole currentUserRole)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Invalid username");

        try
        {
            var res = await _usersDataController.GetUserByUsername(username);
            if (res == null)
                throw new NotFoundException("User was not found");

            if (currentUserRole < UserRole.Manager && res.Id != currentUserId)
                throw new ForbiddenException();

            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByEmail(string email, Guid currentUserId, UserRole currentUserRole)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Invalid email");

        try
        {
            var res = await _usersDataController.GetUserByEmail(email);
            if (res == null)
                throw new NotFoundException("User was not found");

            if (currentUserRole < UserRole.Manager && res.Id != currentUserId)
                throw new ForbiddenException();

            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByLoginParameter(string loginParameter, Guid currentUserId, UserRole currentUserRole)
    {
        if (string.IsNullOrWhiteSpace(loginParameter))
            throw new ValidationException("Invalid login parameter");

        try
        {
            var res = await _usersDataController.GetUserByLoginParameter(loginParameter);
            if (res == null)
                throw new NotFoundException("User was not found");

            if (currentUserRole < UserRole.Manager && res.Id != currentUserId)
                throw new ForbiddenException();

            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateUser(CreateUserRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new ValidationException("Username is required");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");

        try
        {
            var existingByUsername = await _usersDataController.GetUserByUsername(request.UserName);
            if (existingByUsername != null)
                throw new ConflictException("Username is already taken");

            var existingByEmail = await _usersDataController.GetUserByEmail(request.Email);
            if (existingByEmail != null)
                throw new ConflictException("Email is already registered");

            var salt = _passwordHelperService.GenerateSalt();
            var hash = _passwordHelperService.HashPassword(salt, request.Password);

            var user = new User
            {
                Id       = request.Id,
                UserName = request.UserName,
                Email    = request.Email,
                Password = hash,
                UserRole = UserRole.User,
                Name     = request.Name
            };

            var created = await _usersDataController.CreateUser(new UserEntity(user));
            if (!created)
                return false;

            return await _userSecretsDataController.Create(new UserSecretsEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.Id,
                PasswordSalt = salt
            });
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CreateGoogleUserAsync(string email, string googleName, string? avatarUrl = null)
        => await CreateOAuthUserAsync(email, googleName, OAuthProvider.Google, avatarUrl);

    public async Task<bool> CreateGitHubUserAsync(string email, string githubName, string? avatarUrl = null)
        => await CreateOAuthUserAsync(email, githubName, OAuthProvider.GitHub, avatarUrl);

    private async Task<bool> CreateOAuthUserAsync(string email, string displayName, OAuthProvider provider, string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required");

        try
        {
            var existingByEmail = await _usersDataController.GetUserByEmail(email);
            if (existingByEmail != null)
            {
                await EnsureUserSecretsAsync(existingByEmail.Id);
                return existingByEmail.OAuthProvider == provider;
            }

            var baseUsername = email.Split('@')[0].Replace('.', '_');

            if (baseUsername.Length < 2)
                baseUsername = "user";

            var username = baseUsername;
            var existingByUsername = await _usersDataController.GetUserByUsername(username);
            if (existingByUsername != null)
                username = $"{baseUsername}_{Guid.NewGuid().ToString("N")[..4]}";

            var salt = _passwordHelperService.GenerateSalt();
            var hash = _passwordHelperService.HashPassword(salt, Guid.NewGuid().ToString());

            var userId = Guid.NewGuid();

            var resolvedAvatar = IsRealAvatarUrl(avatarUrl) ? avatarUrl : null;
            var resolvedName   = string.IsNullOrWhiteSpace(displayName) ? null : displayName;

            var user = new User
            {
                Id            = userId,
                UserName      = username,
                Email         = email,
                Password      = hash,
                UserRole      = UserRole.User,
                OAuthProvider = provider,
                Name          = resolvedName,
                Avatar        = resolvedAvatar
            };

            await _usersDataController.CreateUser(new UserEntity(user));

            // sp_Users_Create uses SET NOCOUNT ON so AddRecord may return -1 even on success.
            // Verify by reading back from DB instead of trusting the rows-affected value.
            var created = await _usersDataController.GetUserByEmail(email);
            if (created == null) return false;

            await EnsureUserSecretsAsync(created.Id, salt);
            return true;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    private async Task EnsureUserSecretsAsync(Guid userId, string? knownSalt = null)
    {
        var existing = await _userSecretsDataController.GetByUserId(userId);
        if (existing != null) return;

        var salt = knownSalt ?? _passwordHelperService.GenerateSalt();
        await _userSecretsDataController.Create(new UserSecretsEntity
        {
            Id           = Guid.NewGuid(),
            UserId       = userId,
            PasswordSalt = salt
        });
    }

    private static bool IsRealAvatarUrl(string? url) =>
        !string.IsNullOrWhiteSpace(url) && url.Length > 1 &&
        (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
         url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
         url.StartsWith("data:", StringComparison.OrdinalIgnoreCase));

    public async Task<bool> UpdateUser(UpdateUserRequestDto request, Guid currentUserId)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            if (currentUserId != request.Id)
                throw new ForbiddenException();

            var existing = await _usersDataController.GetUserById(request.Id);
            if (existing == null)
                throw new NotFoundException("User was not found");

            string passwordHash = existing.Password ?? throw new ValidationException("User password is missing");
            bool passwordChanged = !string.IsNullOrWhiteSpace(request.Password);

            if (passwordChanged)
            {
                var newSalt = _passwordHelperService.GenerateSalt();
                passwordHash = _passwordHelperService.HashPassword(newSalt, request.Password!);

                var updatedEntity = new UserEntity
                {
                    Id       = request.Id,
                    UserName = request.UserName,
                    Email    = request.Email,
                    UserRole = existing.UserRole,
                    Password = passwordHash,
                    Avatar   = existing.Avatar,
                    Name     = request.Name
                };

                var result = await _usersDataController.UpdateUser(updatedEntity);
                if (!result)
                    return false;

                await _userSecretsDataController.UpdatePasswordSalt(request.Id, newSalt);

                bool isSelf = currentUserId == request.Id;
                var desc = isSelf ? "updated own profile" : $"updated profile of {existing.UserName}";
                _ = _activityLogsService.LogActivity(null, currentUserId, ActivityType.UserUpdated, desc);
                return true;
            }
            else
            {
                var updatedEntity = new UserEntity
                {
                    Id       = request.Id,
                    UserName = request.UserName,
                    Email    = request.Email,
                    UserRole = existing.UserRole,
                    Password = passwordHash,
                    Avatar   = existing.Avatar,
                    Name     = request.Name
                };

                var result = await _usersDataController.UpdateUser(updatedEntity);
                if (result)
                {
                    bool isSelf = currentUserId == request.Id;
                    var desc = isSelf ? "updated own profile" : $"updated profile of {existing.UserName}";
                    _ = _activityLogsService.LogActivity(null, currentUserId, ActivityType.UserUpdated, desc);
                }
                return result;
            }
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> ChangeUserRole(Guid userId, UserRole newRole, Guid currentUserId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            var existing = await _usersDataController.GetUserById(userId);
            if (existing == null)
                throw new NotFoundException("User was not found");

            existing.UserRole = newRole;
            var result = await _usersDataController.UpdateUser(existing);
            if (result)
                _ = _activityLogsService.LogActivity(null, currentUserId, ActivityType.UserRoleChanged,
                    $"changed role of {existing.UserName} to {newRole}");
            return result;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateAvatar(Guid userId, string avatar, Guid currentUserId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (string.IsNullOrWhiteSpace(avatar))
            throw new ValidationException("Avatar is required");
        if (currentUserId != userId)
            throw new ForbiddenException();

        try
        {
            var existing = await _usersDataController.GetUserById(userId);
            if (existing == null)
                throw new NotFoundException("User was not found");

            var result = await _usersDataController.UpdateUserAvatar(userId, avatar);
            if (result)
                _ = _activityLogsService.LogActivity(null, currentUserId, ActivityType.UserUpdated, "updated profile avatar");
            return result;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            var existing = await _usersDataController.GetUserById(userId);
            if (existing == null)
                throw new NotFoundException("User was not found");

            return await _usersDataController.DeleteUser(userId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

}
