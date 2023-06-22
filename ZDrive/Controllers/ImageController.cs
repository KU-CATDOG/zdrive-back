using Microsoft.AspNetCore.Mvc;
using ZDrive.Data;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly ZDriveDbContext _context;

    private List<string> supportedExtensionList = new List<string>()
    {
        ".gif",
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public ImageController(ZDriveDbContext context)
    {
        _context = context;
    }

    [Route("upload")]
    [HttpPost]
    public async Task<IResult> PostImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file is provided.");
        }

        var fileExtension = "." + file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);

        if (!supportedExtensionList.Contains(fileExtension))
        {
            return Results.BadRequest("Not supported extension.");
        }

        if (file.Length > 4194304)
        {
            return Results.BadRequest("File size is too large.");
        }

        var filePath = Path.Combine($"wwwroot{Path.DirectorySeparatorChar}images",
            Path.GetRandomFileName().Substring(0, 8) + fileExtension);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Results.Ok(filePath);
    }
}
