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
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                Password = hash,
                UserRole = UserRole.User
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

    public async Task<bool> CreateGoogleUserAsync(string email, string googleName)
        => await CreateOAuthUserAsync(email, googleName, OAuthProvider.Google);

    public async Task<bool> CreateGitHubUserAsync(string email, string githubName)
        => await CreateOAuthUserAsync(email, githubName, OAuthProvider.GitHub);

    private async Task<bool> CreateOAuthUserAsync(string email, string displayName, OAuthProvider provider)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required");

        try
        {
            var existingByEmail = await _usersDataController.GetUserByEmail(email);
            if (existingByEmail != null)
                return existingByEmail.OAuthProvider == provider;

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
            var user = new User
            {
                Id = userId,
                UserName = username,
                Email = email,
                Password = hash,
                UserRole = UserRole.User,
                OAuthProvider = provider
            };

            var created = await _usersDataController.CreateUser(new UserEntity(user));
            if (!created) return false;

            return await _userSecretsDataController.Create(new UserSecretsEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
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
                    Id = request.Id,
                    UserName = request.UserName,
                    Email = request.Email,
                    UserRole = existing.UserRole,
                    Password = passwordHash
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
                    Id = request.Id,
                    UserName = request.UserName,
                    Email = request.Email,
                    UserRole = existing.UserRole,
                    Password = passwordHash
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
