using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Models;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ZDriveDbContext _context;

    public UserController(ZDriveDbContext context)
    {
        _context = context;
    }

    [Route("project")]
    [HttpGet("project/{num}")]
    public async Task<IResult> ReadContributedProjects(string num)
    {
        var projectIds = await (from m in _context.Members
                                where m.StudentNumber == num
                                select m.ProjectId).ToListAsync();

        var projects = _context.Projects.Where(p => projectIds.Contains(p.Id));

        var ret = await projects.ToListAsync();
        return ret.Count > 0 ? Results.Ok(ret) : Results.NotFound();
    }

    [Route("project")]
    [HttpGet]
    public async Task<IResult> ReadOwnedProjects()
    {
        var projects = from p in _context.Projects
                       select p;

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid != null)
        {
            projects = projects
                .Where(p => p.UserId == int.Parse(sid));
        }

        var ret = await projects.ToListAsync();
        return ret.Count > 0 ? Results.Ok(ret) : Results.NotFound();
    }

    [HttpPut]
    public async Task<IResult> Update(UserInfo user)
    {
        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();

        var _user = await _context.Users.FindAsync(int.Parse(sid));
        if (_user is null) return Results.NotFound();

        if (_user.Name != user.Name)
        {
            var _studentNum = await _context.StudentNums.FindAsync(_user.StudentNumber);
            if (_studentNum is null) return Results.NotFound("StudentNumber was not found.");

            _user.Name = user.Name;
            _studentNum.Name = user.Name;
        }

        await _context.SaveChangesAsync();
        return Results.Created($"/calendar/{_user.Id}", user);
    }
}
