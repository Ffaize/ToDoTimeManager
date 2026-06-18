using System.Net.Http.Headers;
using ToDoTimeManager.Shared.DTOs.Files;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class FilesService : BaseHttpService
{
    private readonly ILogger<FilesService> _logger;

    public FilesService(IHttpClientFactory httpClientFactory, ILogger<FilesService> logger) : base(httpClientFactory)
    {
        ApiControllerName = "Files";
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> GetFilesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(Url());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Dictionary<string, string>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching file list");
            return [];
        }
    }

    /// <summary>
    /// Uploads a file to the API via multipart/form-data.
    /// The API saves it to wwwroot/uploads/ and returns the accessible URL.
    /// </summary>
    public async Task<FileUploadResponseDto?> UploadFileAsync(string fileName, Stream content, string contentType)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync(Url("upload"), form);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FileUploadResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file '{FileName}'", fileName);
            return null;
        }
    }
}
