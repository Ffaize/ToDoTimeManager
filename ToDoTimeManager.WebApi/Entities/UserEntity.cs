using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Entities
{
    public class UserEntity
    {
        public required Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserRole? UserRole { get; set; }

    }
}
