using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IProjectsDataController
{
    Task<List<ProjectEntity>> GetAllProjects();
    Task<ProjectEntity?> GetProjectById(Guid projectId);
    Task<List<ProjectEntity>> GetProjectsByUserId(Guid userId);
    Task<bool> CreateProject(ProjectEntity newProject);
    Task<bool> UpdateProject(ProjectEntity updatedProject);
    Task<bool> DeleteProject(Guid projectId);
}