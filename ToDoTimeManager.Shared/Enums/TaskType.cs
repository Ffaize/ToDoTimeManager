namespace ToDoTimeManager.Shared.Enums;

/// <summary>
/// Classifies a to-do item by work type.
/// Stored as INT in the database; the column is nullable for backwards compatibility.
/// </summary>
public enum TaskType
{
    UserStory = 0,
    Feature   = 1,
    Bug       = 2,
    Incident  = 3,
    Support   = 4,
    Meet      = 5
}
