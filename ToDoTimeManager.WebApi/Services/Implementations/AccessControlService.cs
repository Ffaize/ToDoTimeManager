using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class AccessControlService : IAccessControlService
{
    private readonly IProjectsDataController _projectsDataController;
    private readonly IProjectTeamsDataController _projectTeamsDataController;
    private readonly ITeamMembersDataController _teamMembersDataController;
    private readonly IToDosDataController _toDosDataController;
    private readonly ITimeLogsDataController _timeLogsDataController;
    private readonly ILogger<AccessControlService> _logger;

    public AccessControlService(
        IProjectsDataController projectsDataController,
        IProjectTeamsDataController projectTeamsDataController,
        ITeamMembersDataController teamMembersDataController,
        IToDosDataController toDosDataController,
        ITimeLogsDataController timeLogsDataController,
        ILogger<AccessControlService> logger)
    {
        _projectsDataController = projectsDataController;
        _projectTeamsDataController = projectTeamsDataController;
        _teamMembersDataController = teamMembersDataController;
        _toDosDataController = toDosDataController;
        _timeLogsDataController = timeLogsDataController;
        _logger = logger;
    }

    public async Task<bool> CanAccessProject(Guid userId, Guid projectId)
    {
        try
        {
            ProjectEntity? project = await _projectsDataController.GetProjectById(projectId);
            if (project == null) return false;

            if (project.CreatedBy == userId) return true;

            List<ProjectTeamEntity> projectTeams = await _projectTeamsDataController.GetTeamsByProjectId(projectId);

            foreach (ProjectTeamEntity pt in projectTeams)
            {
                if (pt.ProjectManagerId == userId) return true;

                TeamMemberEntity? membership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(pt.TeamId, userId);
                if (membership != null) return true;
            }

            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessToDo(Guid userId, Guid toDoId)
    {
        try
        {
            ToDoEntity? todo = await _toDosDataController.GetToDoById(toDoId);
            if (todo == null) return false;

            if (todo.AssignedTo == userId) return true;

            if (todo.TeamId.HasValue)
            {
                TeamMemberEntity? membership = await _teamMembersDataController.GetMemberByTeamIdAndUserId(todo.TeamId.Value, userId);
                if (membership != null) return true;
            }

            if (todo.ProjectId.HasValue)
                return await CanAccessProject(userId, todo.ProjectId.Value);

            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessTimeLog(Guid userId, Guid timeLogId)
    {
        try
        {
            TimeLogEntity? timeLog = await _timeLogsDataController.GetTimeLogById(timeLogId);
            if (timeLog == null) return false;

            if (timeLog.UserId == userId) return true;

            return await CanAccessToDo(userId, timeLog.ToDoId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
