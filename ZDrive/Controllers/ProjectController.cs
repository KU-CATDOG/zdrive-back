using Microsoft.AspNetCore.Mvc;
using ZDrive.Data;
using ZDrive.Services;

namespace ZDrive.Controllers;

public class ProjectController : ControllerBase
{
    private readonly IAuthorizationManager _auth;
    private readonly ZDriveDbContext _context;

    public ProjectController(IAuthorizationManager auth, ZDriveDbContext context)
    {
        _auth = auth;
        _context = context;
    }
}