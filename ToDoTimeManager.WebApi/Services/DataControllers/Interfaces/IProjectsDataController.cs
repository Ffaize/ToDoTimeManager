using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface IProjectsDataController
{
    Task<List<ProjectEntity>> GetAllProjects();
    Task<ProjectEntity?> GetProjectById(Guid projectId);
    Task<List<ProjectEntity>> GetProjectsByUserId(Guid userId);
    Task<bool> CreateProject(ProjectEntity newProject);
    Task<bool> UpdateProject(ProjectEntity updatedProject);
    Task<bool> DeleteProject(Guid projectId);
}