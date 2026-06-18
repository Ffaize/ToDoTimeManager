using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;
using ToDoTimeManager.WebApi.AdditionalComponents;
using ToDoTimeManager.WebApi.Hubs;
using ToDoTimeManager.WebApi.Middleware;
using ToDoTimeManager.WebApi.Seeders;
using ToDoTimeManager.WebApi.Services.Implementations;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.DataAccess.DbAccessServices;
using ToDoTimeManager.DataAccess.DataControllers.Implementation;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Utils.Implementations;
using ToDoTimeManager.Business.Utils.Interfaces;
using ToDoTimeManager.Business.Services.Implementations;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.ServiceDefaults;

namespace ToDoTimeManager.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        VerifyJwtKey(builder);
        AddServices(builder);
        AddRateLimit(builder);
        AddSwaggerGeneration(builder);
        AddAuth(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapDefaultEndpoints();
        app.UseMiddleware<GlobalExceptionHandler>();
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<CommunicationHub>("/hubs/communication").RequireAuthorization();

        //await DataSeeder.SeedAsync(app.Services);

        await app.RunAsync();
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddScoped<IHubNotifier, HubNotifier>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();


        // Add utils to the container.
        builder.Services.AddScoped<IPasswordHelperService, PasswordHelperService>();
        builder.Services.AddScoped<IJwtGeneratorService, JwtGeneratorService>();

        builder.Services.AddScoped<IDbAccessService, DbAccessService>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IUsersDataController, UsersDataController>();
        builder.Services.AddScoped<IUserSettingsDataController, UserSettingsDataController>();
        builder.Services.AddScoped<IUserTotpSecretsDataController, UserTotpSecretsDataController>();
        builder.Services.AddScoped<IAccessControlDataController, AccessControlDataController>();
        builder.Services.AddScoped<IToDosDataController, ToDosDataController>();
        builder.Services.AddScoped<ITimeLogsDataController, TimeLogsDataController>();
        builder.Services.AddScoped<ITeamsDataController, TeamsDataController>();
        builder.Services.AddScoped<ITeamMembersDataController, TeamMembersDataController>();
        builder.Services.AddScoped<IProjectsDataController, ProjectsDataController>();
        builder.Services.AddScoped<IProjectTeamsDataController, ProjectTeamsDataController>();
        builder.Services.AddScoped<IActivityLogsDataController, ActivityLogsDataController>();

        builder.Services.AddScoped<IUsersService, UsersService>();
        builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
        builder.Services.AddScoped<IToDosService, ToDosService>();
        builder.Services.AddScoped<ITimeLogsService, TimeLogsService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IStatisticService, StatisticService>();
        builder.Services.AddScoped<ITeamsService, TeamsService>();
        builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IActivityLogsService, ActivityLogsService>();
        builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

        builder.Services.AddScoped<IUserSecretsDataController, UserSecretsDataController>();
        builder.Services.AddScoped<ITwoFactorCodesDataController, TwoFactorCodesDataController>();
        builder.Services.AddScoped<IPasswordResetsDataController, PasswordResetsDataController>();
        builder.Services.AddScoped<ITwoFactorCodesHelper, TwoFactorCodesHelper>();
        builder.Services.AddSingleton<ITotpService, TotpService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

        builder.Services.AddScoped<GlobalExceptionHandler>();
        builder.Services.AddMemoryCache();
    }

    private static void VerifyJwtKey(WebApplicationBuilder builder)
    {
        var jwtKey = builder.Configuration["JwtSettings:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException(
                "JwtSettings:Key is not configured. Set it via dotnet user-secrets or the JwtSettings__Key environment variable.");
    }

    private static void AddAuth(WebApplicationBuilder builder)
    {
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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) &&
                            context.Request.Path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie("ExternalCookieScheme", options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration["GoogleAuth:ClientId"] ?? string.Empty;
                options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"] ?? string.Empty;
                options.CallbackPath = "/Auth/GoogleCallback";
                options.SignInScheme = "ExternalCookieScheme";
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = ctx =>
                    {
                        if (ctx.User.TryGetProperty("picture", out var pic))
                        {
                            var url = pic.GetString();
                            if (!string.IsNullOrEmpty(url))
                                ctx.Identity?.AddClaim(new System.Security.Claims.Claim("picture", url));
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGitHub(GitHubAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration["GitHubAuth:ClientId"] ?? string.Empty;
                options.ClientSecret = builder.Configuration["GitHubAuth:ClientSecret"] ?? string.Empty;
                options.CallbackPath = "/Auth/GitHubCallback";
                options.SignInScheme = "ExternalCookieScheme";
                options.Scope.Add("user:email");
            });
    }

    private static void AddSwaggerGeneration(WebApplicationBuilder builder)
    {
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
    }

    private static void AddRateLimit(WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                context.HttpContext.Response.ContentType = "application/problem+json";
                await context.HttpContext.Response.WriteAsync(
                    """{"type":"https://tools.ietf.org/html/rfc6585#section-4","title":"Too Many Requests","status":429,"detail":"Rate limit exceeded. Please try again later."}""",
                    ct);
            };

            options.AddSlidingWindowLimiter("auth-login", o =>
            {
                o.PermitLimit = 5;
                o.Window = TimeSpan.FromSeconds(60);
                o.SegmentsPerWindow = 2;
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });

            options.AddSlidingWindowLimiter("auth-send-code", o =>
            {
                o.PermitLimit = 3;
                o.Window = TimeSpan.FromSeconds(300);
                o.SegmentsPerWindow = 5;
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });

            options.AddSlidingWindowLimiter("auth-verify-code", o =>
            {
                o.PermitLimit = 5;
                o.Window = TimeSpan.FromSeconds(60);
                o.SegmentsPerWindow = 2;
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });

            options.AddSlidingWindowLimiter("auth-register", o =>
            {
                o.PermitLimit = 5;
                o.Window = TimeSpan.FromSeconds(60);
                o.SegmentsPerWindow = 2;
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });

            options.AddSlidingWindowLimiter("auth-forgot-password", o =>
            {
                o.PermitLimit = 3;
                o.Window = TimeSpan.FromSeconds(300);
                o.SegmentsPerWindow = 5;
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });

            options.AddSlidingWindowLimiter("auth-reset-password", o =>
            {
                o.PermitLimit = 5;
                o.Window = TimeSpan.FromSeconds(60);
                o.SegmentsPerWindow = 2;
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit = 0;
            });
        });
    }
}