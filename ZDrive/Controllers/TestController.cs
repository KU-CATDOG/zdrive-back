using Microsoft.AspNetCore.Mvc;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string TestGet() => "Hello World!!";
}