using Microsoft.AspNetCore.Components.Forms;
using ToDoTimeManager.WebUI.Services.HttpServices;
using ToDoTimeManager.WebUI.Services.Services.Interfaces;

namespace ToDoTimeManager.WebUI.Components.PageComponents.FilesPage;

public partial class FileManagerComponent
{
    [Inject] private FilesService FilesService { get; set; } = null!;
    [Inject] private IToastsService ToastsService { get; set; } = null!;
    [Inject] private ILogger<FileManagerComponent> Logger { get; set; } = null!;

    private Dictionary<string, string> _files = [];
    private bool _uploading;

    protected override async Task OnInitializedAsync()
    {
        await Loading(LoadFilesAsync);
    }

    private async Task LoadFilesAsync()
    {
        _files = await FilesService.GetFilesAsync();
    }

    private async Task HandleFileSelectedAsync(InputFileChangeEventArgs e)
    {
        _uploading = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            var file = e.File;
            var uploadInfo = await FilesService.GetUploadUrlAsync(file.Name);
            if (uploadInfo == null)
            {
                ToastsService.ShowError(Localizer["Failed to get upload URL."]);
                return;
            }

            var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType;

            await using var stream = file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024);
            var success = await FilesService.UploadToSasUrlAsync(
                uploadInfo.UploadUrl, stream, contentType, file.Size);

            if (success)
            {
                ToastsService.ShowSuccess(Localizer["File uploaded successfully."]);
                await LoadFilesAsync();
            }
            else
            {
                ToastsService.ShowError(Localizer["Upload failed."]);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during file upload");
            ToastsService.ShowError(Localizer["Upload failed."]);
        }
        finally
        {
            _uploading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private static bool IsVideoFile(string blobName)
    {
        var ext = Path.GetExtension(blobName).ToLowerInvariant();
        return ext is ".mp4" or ".webm" or ".ogg" or ".mov" or ".avi" or ".mkv";
    }

    private static string GetDisplayName(string blobName)
    {
        // blobName format: {guid}_{originalFileName}
        var idx = blobName.IndexOf('_');
        return idx >= 0 && idx < blobName.Length - 1 ? blobName[(idx + 1)..] : blobName;
    }
}
