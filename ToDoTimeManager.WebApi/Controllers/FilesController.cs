using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs.Files;

namespace ToDoTimeManager.WebApi.Controllers;

[Authorize]
public class FilesController : BaseController
{
    private const string UploadsFolderName = "uploads";
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IWebHostEnvironment env, ILogger<FilesController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetFiles()
    {
        var uploadsPath = GetUploadsPath();
        if (!Directory.Exists(uploadsPath))
            return Ok(new Dictionary<string, string>());

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var files = Directory.GetFiles(uploadsPath)
            .ToDictionary(
                f => Path.GetFileName(f),
                f => $"{baseUrl}/{UploadsFolderName}/{Uri.EscapeDataString(Path.GetFileName(f))}"
            );

        return Ok(files);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    public async Task<ActionResult<FileUploadResponseDto>> UploadFile(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided");

        var uploadsPath = GetUploadsPath();
        Directory.CreateDirectory(uploadsPath);

        // Prefix with GUID to avoid collisions and path traversal
        var safeOriginalName = Path.GetFileName(file.FileName);
        var storedName = $"{Guid.NewGuid()}_{safeOriginalName}";
        var filePath = Path.Combine(uploadsPath, storedName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var url = $"{baseUrl}/{UploadsFolderName}/{Uri.EscapeDataString(storedName)}";

        _logger.LogInformation("Saved file '{StoredName}' ({Bytes} bytes)", storedName, file.Length);
        return Ok(new FileUploadResponseDto(storedName, url));
    }

    private string GetUploadsPath()
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        return Path.Combine(webRoot, UploadsFolderName);
    }
}
