using ToDoTimeManager.Shared.DTOs.Project;
using ToDoTimeManager.Shared.DTOs.Team;
using ToDoTimeManager.Shared.DTOs.User;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Shared.Extensions;

public static class MappingExtensions
{
    public static UserResponseDto ToResponseDto(this User user) =>
        new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            UserRole = user.UserRole
        };

    public static TeamResponseDto ToResponseDto(this Team team, List<TeamMember>? members) =>
        new()
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            CreatedAt = team.CreatedAt,
            CreatedBy = team.CreatedBy,
            MemberCount = team.MemberCount,
            Members = members
        };

    public static ProjectResponseDto ToResponseDto(this Project project, List<ProjectTeam>? teams) =>
        new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            CreatedBy = project.CreatedBy,
            TeamCount = project.TeamCount,
            Type = project.Type,
            Teams = teams
        };
}
