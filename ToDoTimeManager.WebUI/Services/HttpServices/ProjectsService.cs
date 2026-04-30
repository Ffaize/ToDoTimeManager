using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class ProjectsService : BaseHttpService
{
    private readonly ILogger<ProjectsService> _logger;

    public ProjectsService(IHttpClientFactory httpClientFactory, ILogger<ProjectsService> logger) : base(httpClientFactory)
    {
        ApiControllerName = "Projects";
        _logger = logger;
    }

    public async Task<List<ProjectResponseDto>> GetAllProjects()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url("GetAll"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProjectResponseDto>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all projects");
            return [];
        }
    }

    public async Task<ProjectResponseDto?> GetProjectById(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"GetById/{id}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProjectResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching project by ID: {ProjectId}", id);
            return null;
        }
    }

    public async Task<List<ToDo>> GetToDosByProjectId(Guid projectId)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"GetToDosByProjectId/{projectId}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ToDo>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todos for project ID: {ProjectId}", projectId);
            return [];
        }
    }

    public async Task<List<ProjectResponseDto>> GetMyProjects()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url("GetMyProjects"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProjectResponseDto>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user's projects");
            return [];
        }
    }

    public async Task<bool> CreateProject(CreateProjectRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("Create"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return false;
        }
    }

    public async Task<bool> UpdateProject(UpdateProjectRequestDto request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(Url("Update"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project");
            return false;
        }
    }

    public async Task<bool> AddTeam(ProjectTeamUpsertRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("AddTeam"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding team to project");
            return false;
        }
    }

    public async Task<bool> RemoveTeam(Guid projectId, Guid teamId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(Url($"RemoveTeam/{projectId}/{teamId}"));
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing team {TeamId} from project {ProjectId}", teamId, projectId);
            return false;
        }
    }

    public async Task<bool> DeleteProject(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(Url($"Delete/{id}"));
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project with ID: {ProjectId}", id);
            return false;
        }
    }
}
