using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Seeders;

public static class DataSeeder
{
    private static readonly Guid SeedUserId = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private const string SeedUserEmail = "dmitro.danko@gmail.com";
    private const string SeedUserName = "Ffaize";
    private const string SeedUserPassword = "Password1!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var usersData = scope.ServiceProvider.GetRequiredService<IUsersDataController>();
        var userSecretsData = scope.ServiceProvider.GetRequiredService<IUserSecretsDataController>();
        var passwordHelper = scope.ServiceProvider.GetRequiredService<IPasswordHelperService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var existing = await usersData.GetUserByEmail(SeedUserEmail);
        if (existing != null)
        {
            logger.LogInformation("Seed user already exists, skipping.");
            return;
        }

        var salt = passwordHelper.GenerateSalt();
        var user = new UserEntity
        {
            Id = SeedUserId,
            UserName = SeedUserName,
            Email = SeedUserEmail,
            Password = passwordHelper.HashPassword(salt, SeedUserPassword),
            UserRole = UserRole.Admin
        };

        var created = await usersData.CreateUser(user);
        if (!created)
        {
            logger.LogError("Failed to create seed user.");
            return;
        }

        await userSecretsData.Create(new UserSecretsEntity
        {
            Id = Guid.NewGuid(),
            UserId = SeedUserId,
            PasswordSalt = salt
        });

        logger.LogInformation("Seed user created: {Email}", SeedUserEmail);
    }
}
