using Microsoft.AspNetCore.Mvc;
using ZDrive.Data;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly ZDriveDbContext _context;

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
            return Results.BadRequest("파일이 제공되지 않았습니다.");
        }
        
        var fileExtension = "." + file.ContentType.Substring(file.ContentType.IndexOf("/") + 1);
        var filePath = Path.Combine(@"wwwroot\images", 
            Path.GetRandomFileName().Substring(0, 8) + fileExtension);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        return Results.Ok(filePath);
    }
}