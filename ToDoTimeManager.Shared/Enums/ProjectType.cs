namespace ToDoTimeManager.Shared.Enums;

/// <summary>
/// Classifies a project by its primary domain.
/// Stored as INT in the database; the column is nullable for backwards compatibility.
/// </summary>
public enum ProjectType
{
    Backend     = 0,
    Frontend    = 1,
    DataBase    = 2,
    FullStack   = 3,
    Mobile      = 4,
    DevOps      = 5,
    DataScience = 6,
    Security    = 7,
    Other       = 8
}
