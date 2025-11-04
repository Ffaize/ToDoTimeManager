using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IPasswordHelperService _passwordHelperService;
        private readonly IJwtGeneratorService _jwtGeneratorService;
        private readonly IConfiguration _configuration;

        public AuthService(ILogger<AuthService> logger, IPasswordHelperService passwordHelperService, IJwtGeneratorService jwtGeneratorService, IConfiguration configuration)
        {
            _logger = logger;
            _passwordHelperService = passwordHelperService;
            _jwtGeneratorService = jwtGeneratorService;
            _configuration = configuration;
        }

        public TokenModel? AuthenticateUser(LoginUser loginUser, User user)
        {
            try
            {
                if (_passwordHelperService.VerifyPassword(user,
                _passwordHelperService.HashPassword(user.Id.ToString(), loginUser.Password!)))
                    return null;

                return new TokenModel()
                {
                    AccessToken = _jwtGeneratorService.GenerateAccessToken(user.Id.ToString(), user.UserRole),
                    RefreshToken = _jwtGeneratorService.GenerateRefreshToken(),
                    RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenLifetime"] ?? "7"))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public TokenModel? RefreshAuthToken(TokenModel tokenModel)
        {
            try
            {
                var (userId, userRole) = _jwtGeneratorService.GetUserDataFromAccessToken(tokenModel.AccessToken!);
                if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userRole))
                    return null;

                return new TokenModel()
                {
                    AccessToken = _jwtGeneratorService.GenerateAccessToken(userId, Enum.Parse<UserRole>(userRole)),
                    RefreshToken = tokenModel.RefreshToken,
                    RefreshTokenExpiresAt = tokenModel.RefreshTokenExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
