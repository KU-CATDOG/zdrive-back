using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ZDriveDbContext _context;

    public TestController(ZDriveDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public void TestGet()
    {
    }
}