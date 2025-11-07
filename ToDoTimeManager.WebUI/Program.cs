using ToDoTimeManager.WebUI.Services.CircuitServicesAccesor;
using ToDoTimeManager.WebUI.Services.Implementations;
using ToDoTimeManager.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ToDoTimeManager.WebUI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddCircuitServicesAccesor();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddLocalization();
        builder.Services.AddServerSideBlazor()
            .AddCircuitOptions(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromDays(1);
            });

        builder.Services.AddScoped<IToastMessagesService, ToastMessagesService>();


        var app = builder.Build();

        string[] supportedCultures = ["en-US", "uk-UA"];
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[1])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

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
        app.MapBlazorHub();
        app.MapControllers();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}