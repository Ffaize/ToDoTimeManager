using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ToDoTimeManager.WebApi.AdditionalComponents;
using ToDoTimeManager.WebApi.Middleware;
using ToDoTimeManager.WebApi.Seeders;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Implementation;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Implementations;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Implementations;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();


        // Add utils to the container.
        builder.Services.AddScoped<IPasswordHelperService, PasswordHelperService>();
        builder.Services.AddScoped<IJwtGeneratorService, JwtGeneratorService>();

        builder.Services.AddScoped<IDbAccessService, DbAccessService>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IUsersDataController, UsersDataController>();
        builder.Services.AddScoped<IAccessControlDataController, AccessControlDataController>();
        builder.Services.AddScoped<IToDosDataController, ToDosDataController>();
        builder.Services.AddScoped<ITimeLogsDataController, TimeLogsDataController>();
        builder.Services.AddScoped<ITeamsDataController, TeamsDataController>();
        builder.Services.AddScoped<ITeamMembersDataController, TeamMembersDataController>();
        builder.Services.AddScoped<IProjectsDataController, ProjectsDataController>();
        builder.Services.AddScoped<IProjectTeamsDataController, ProjectTeamsDataController>();
        builder.Services.AddScoped<IActivityLogsDataController, ActivityLogsDataController>();

        builder.Services.AddScoped<IUsersService, UsersService>();
        builder.Services.AddScoped<IToDosService, ToDosService>();
        builder.Services.AddScoped<ITimeLogsService, TimeLogsService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IStatisticService, StatisticService>();
        builder.Services.AddScoped<ITeamsService, TeamsService>();
        builder.Services.AddScoped<IProjectsService, ProjectsService>();
        builder.Services.AddScoped<IAccessControlService, AccessControlService>();
        builder.Services.AddScoped<IActivityLogsService, ActivityLogsService>();
        builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

        builder.Services.AddScoped<ITwoFactorCodesDataController, TwoFactorCodesDataController>();
        builder.Services.AddScoped<ITwoFactorCodeGeneratorService, TwoFactorCodeGeneratorService>();
        builder.Services.AddScoped<IEmailService, EmailService>();

        builder.Services.AddScoped<GlobalExceptionHandler>();


        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "Enter ONLY your valid token in the text input below.\n\nExample: \"eyJhbGciOiJIUzI1NiIsInR...\""
            });
            options.OperationFilter<AuthResponsesOperationFilter>();
        });
        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? string.Empty)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<GlobalExceptionHandler>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        await DataSeeder.SeedAsync(app.Services);

        app.Run();
    }
}