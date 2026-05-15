using ToDoTimeManager.Shared.DTOs.Team;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class TeamsService : BaseHttpService
{
    private readonly ILogger<TeamsService> _logger;

    public TeamsService(IHttpClientFactory httpClientFactory, ILogger<TeamsService> logger) : base(httpClientFactory)
    {
        ApiControllerName = "Teams";
        _logger = logger;
    }

    public async Task<List<TeamResponseDto>> GetAllTeams()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url("GetAll"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TeamResponseDto>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all teams");
            return [];
        }
    }

    public async Task<TeamResponseDto?> GetTeamById(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"GetById/{id}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TeamResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team by ID: {TeamId}", id);
            return null;
        }
    }

    public async Task<List<ToDo>> GetToDosByTeamId(Guid teamId)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"GetToDosByTeamId/{teamId}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ToDo>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todos for team ID: {TeamId}", teamId);
            return [];
        }
    }

    public async Task<List<TeamResponseDto>> GetMyTeams()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url("GetMyTeams"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TeamResponseDto>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user's teams");
            return [];
        }
    }

    public async Task<bool> CreateTeam(CreateTeamRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("Create"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            return false;
        }
    }

    public async Task<bool> UpdateTeam(UpdateTeamRequestDto request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(Url("Update"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team");
            return false;
        }
    }

    public async Task<bool> AddMember(TeamMemberUpsertRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("AddMember"), request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to team");
            return false;
        }
    }

    public async Task<bool> RemoveMember(Guid teamId, Guid userId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(Url($"RemoveMember/{teamId}/{userId}"));
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member {UserId} from team {TeamId}", userId, teamId);
            return false;
        }
    }

    public async Task<bool> DeleteTeam(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(Url($"Delete/{id}"));
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team with ID: {TeamId}", id);
            return false;
        }
    }
}
