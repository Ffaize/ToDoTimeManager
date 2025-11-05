using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation
{
    public class UsersDataController : IUsersDataController
    {
        private readonly IDbAccessService _dbAccessService;
        private readonly ILogger<UsersDataController> _logger;

        public UsersDataController(IDbAccessService dbAccessService, ILogger<UsersDataController> logger)
        {
            _dbAccessService = dbAccessService;
            _logger = logger;
        }

        public async Task<List<UserEntity>> GetAllUsers()
        {
            try
            {
                return await _dbAccessService.GetAllRecords<UserEntity>("sp_Users_GetAll");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<UserEntity?> GetUserById(Guid userId)
        {
            try
            {
                return await _dbAccessService.GetRecordById<UserEntity>("sp_Users_GetById", userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<UserEntity?> GetUserByUsername(string username)
        {
            try
            {
                return await _dbAccessService.GetOneByParameter<UserEntity>("sp_Users_GetByUsername", "UserName", username);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }
        
        public async Task<UserEntity?> GetUserByEmail(string email)
        {
            try
            {
                return await _dbAccessService.GetOneByParameter<UserEntity>("sp_Users_GetByEmail", "Email", email);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }
        
        public async Task<UserEntity?> GetUserByLoginParameter(string loginParameter)
        {
            try
            {
                return await _dbAccessService.GetOneByParameter<UserEntity>("sp_Users_GetByLoginParameter", "Email", loginParameter);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<bool> CreateUser(UserEntity newUser)
        {
            try
            {
                return await _dbAccessService.AddRecord("sp_Users_Create", newUser) >= 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateUser(UserEntity updatedUser)
        {
            try
            {
                return await _dbAccessService.UpdateRecord("sp_Users_Update", updatedUser) >= 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            try
            {
                return await _dbAccessService.DeleteRecordById("sp_Users_DeleteById", userId) >= 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
    }
}
