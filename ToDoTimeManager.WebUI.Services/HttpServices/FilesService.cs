using System.Net.Http.Headers;
using ToDoTimeManager.Shared.DTOs.Files;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class FilesService : BaseHttpService
{
    private readonly ILogger<FilesService> _logger;
    private readonly HttpClient _blobHttpClient;

    public FilesService(IHttpClientFactory httpClientFactory, ILogger<FilesService> logger) : base(httpClientFactory)
    {
        ApiControllerName = "Files";
        _logger = logger;
        _blobHttpClient = httpClientFactory.CreateClient("AzureBlob");
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

    public async Task<UploadUrlResponseDto?> GetUploadUrlAsync(string fileName)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"upload-url?fileName={Uri.EscapeDataString(fileName)}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UploadUrlResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting upload URL for '{FileName}'", fileName);
            return null;
        }
    }

    /// <summary>
    /// Uploads file content directly to Azure Blob Storage via a pre-signed SAS URL (PUT).
    /// This request bypasses the API and goes straight to the blob endpoint.
    /// </summary>
    public async Task<bool> UploadToSasUrlAsync(string sasUrl, Stream content, string contentType, long contentLength)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, sasUrl);
            var body = new StreamContent(content);
            body.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            body.Headers.ContentLength = contentLength;
            request.Content = body;
            request.Headers.Add("x-ms-blob-type", "BlockBlob");

            var response = await _blobHttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Blob PUT returned {StatusCode}", response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to SAS URL");
            return false;
        }
    }
}
