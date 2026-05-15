using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using ToDoTimeManager.ServiceDefaults;
using ToDoTimeManager.WebUI.Handlers;
using ToDoTimeManager.WebUI.Services.Helpers.CircuitServicesAccesor;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Services.Implementations;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;


namespace ToDoTimeManager.WebUI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddCircuitServicesAccesor();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddLocalization();
        builder.Services.AddServerSideBlazor()
            .AddCircuitOptions(options => { options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromDays(1); });

        builder.Services.AddSingleton<IToastsService, ToastsService>();
        builder.Services.AddSingleton<ITwoFaTimerService, TwoFaTimerService>();
        builder.Services.AddScoped<IModalService, ModalService>();
        builder.Services.AddScoped<TokenRefreshService>();
        builder.Services.AddScoped<TokenMessageHandler>();
        builder.Services.AddScoped<ToastMessageHandler>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<StatisticService>();
        builder.Services.AddScoped<ToDosService>();
        builder.Services.AddScoped<TimeLogsService>();
        builder.Services.AddScoped<TeamsService>();
        builder.Services.AddScoped<ProjectsService>();

        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

        builder.Services.AddHttpClient("TodoTimeManager", client =>
        {
            client.BaseAddress = new Uri("https+http://webapi");
            client.DefaultRequestHeaders.Accept.Clear();
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<ToastMessageHandler>().AddHttpMessageHandler<TokenMessageHandler>();

        var app = builder.Build();

        string[] supportedCultures = ["en-US", "uk-UA"];
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[1])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.MapDefaultEndpoints();
        app.UseRequestLocalization(localizationOptions);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapBlazorHub();
        app.MapControllers();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}