using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs.Files;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
public class FilesController : BaseController
{
    private const string ContainerName = "media";
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<FilesController> _logger;

    public FilesController(BlobServiceClient blobServiceClient, ILogger<FilesController> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<Dictionary<string, string>>> GetFiles()
    {
        try
        {
            var container = await GetContainerAsync();
            var result = new Dictionary<string, string>();

            await foreach (var blobItem in container.GetBlobsAsync())
            {
                var blobClient = container.GetBlobClient(blobItem.Name);
                var lifetime = IsVideoFile(blobItem.Name)
                    ? TimeSpan.FromMinutes(60)
                    : TimeSpan.FromMinutes(30);
                var sasUrl = GenerateSasUrl(blobClient, BlobSasPermissions.Read, lifetime);
                if (sasUrl != null)
                    result[blobItem.Name] = sasUrl;
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files from container '{Container}'", ContainerName);
            return StatusCode(500);
        }
    }

    [HttpGet("upload-url")]
    public async Task<ActionResult<UploadUrlResponseDto>> GetUploadUrl([FromQuery] string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest("fileName is required");

        try
        {
            var container = await GetContainerAsync();
            var blobName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = container.GetBlobClient(blobName);

            var sasUrl = GenerateSasUrl(
                blobClient,
                BlobSasPermissions.Write | BlobSasPermissions.Create,
                TimeSpan.FromMinutes(10));

            if (sasUrl == null)
                return StatusCode(500, "Cannot generate SAS URL — storage client does not support shared-key signing.");

            return Ok(new UploadUrlResponseDto(blobName, sasUrl));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating upload URL for '{FileName}'", fileName);
            return StatusCode(500);
        }
    }

    private async Task<BlobContainerClient> GetContainerAsync()
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None);
        return container;
    }

    private static string? GenerateSasUrl(BlobClient blobClient, BlobSasPermissions permissions, TimeSpan lifetime)
    {
        if (!blobClient.CanGenerateSasUri)
            return null;

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(lifetime)
        };
        sasBuilder.SetPermissions(permissions);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    private static bool IsVideoFile(string blobName)
    {
        var ext = Path.GetExtension(blobName).ToLowerInvariant();
        return ext is ".mp4" or ".webm" or ".ogg" or ".mov" or ".avi" or ".mkv";
    }
}
