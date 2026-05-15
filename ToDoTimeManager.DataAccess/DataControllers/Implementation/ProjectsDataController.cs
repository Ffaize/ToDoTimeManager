using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.DataAccess.DbAccessServices;
using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Implementation;

public class ProjectsDataController : IProjectsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<ProjectsDataController> _logger;

    public ProjectsDataController(IDbAccessService dbAccessService, ILogger<ProjectsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<List<ProjectEntity>> GetAllProjects()
    {
        try
        {
            return await _dbAccessService.GetAllRecords<ProjectEntity>("sp_Projects_GetAll");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ProjectEntity?> GetProjectById(Guid projectId)
    {
        try
        {
            return await _dbAccessService.GetRecordById<ProjectEntity>("sp_Projects_GetById", projectId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<ProjectEntity>> GetProjectsByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetAllByParameter<ProjectEntity>("sp_Projects_GetByUserId", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<bool> CreateProject(ProjectEntity newProject)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_Projects_Create", newProject) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateProject(ProjectEntity updatedProject)
    {
        try
        {
            return await _dbAccessService.UpdateRecord("sp_Projects_Update", updatedProject) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteProject(Guid projectId)
    {
        try
        {
            return await _dbAccessService.DeleteRecordById("sp_Projects_DeleteById", projectId) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}