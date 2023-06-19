using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Extensions;
using ZDrive.Models;
using ZDrive.Services;
using ZDrive.Utils;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase
{
    private readonly ZDriveDbContext _context;

    public ProjectController(ZDriveDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IResult> Create(ProjectInformation project)
    {
        if ((await _context.Projects.FirstOrDefaultAsync(p => p.Name == project.Name)) != null)
            return Results.Conflict();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();

        var newProject = new Project();
        newProject.Copy(project);
        newProject.UserId = int.Parse(sid);

        await _context.Projects.AddAsync(newProject);
        await _context.SaveChangesAsync();

        return Results.Created($"/project/{newProject.Id}", newProject);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var _project = await _context.Projects.FindAsync(id);
        if (_project == null)
            return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _project.UserId.ToString()) return Results.Forbid();

        _context.Projects.Remove(_project);
        await _context.SaveChangesAsync();
        return Results.Ok(_project);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IResult> Read(int id)
    {
        var projects = from p in _context.Projects
            .Include(p => p.Members)
            .ThenInclude(m => m.StudentNum)
            select p;

        var project = await projects.FirstOrDefaultAsync(p => p.Id == id);

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null && (project?.Visibility) != Visibility.Public)
        {
            project = null;
        }

        return project == null ? Results.NotFound() : Results.Ok(project);
    }

    [Route("list")]
    [HttpGet("list/{search}/{period}")]
    [AllowAnonymous]
    public async Task<IResult> ReadAllProject
    (
        [FromQuery(Name = "search")]string? search = null,
        [FromQuery(Name = "period")]string? period = null
    )
    {
        var projects = from p in _context.Projects
            select p;
        
        if (!String.IsNullOrEmpty(search))
        {
            projects = projects
                .Where(p => p.Name.Contains(search));
        }

        if (!String.IsNullOrEmpty(period))
        {
            try
            {
                var date = new Period(period);
                projects = projects
                    .Where
                    (
                        p => p.StartDate != null &&
                        date.Semester == Semester.First ? (new DateTime(date.Year, 3, 1) < p.StartDate && new DateTime(date.Year, 8, 31) > p.StartDate)
                            : ( new DateTime(date.Year, 9, 1) < p.StartDate && new DateTime(date.Year + 1, 2, DateTime.IsLeapYear(date.Year) ? 29 : 28) > p.StartDate )
                    );
            }
            catch
            {
                return Results.BadRequest();
            }
        }

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null)
        {
            projects = projects
                .Where(p => p.Visibility == Visibility.Public);
        }

        return Results.Ok(await projects.ToListAsync());
    }

    [HttpPut("{id}")]
    public async Task<IResult> Update(int id, ProjectInformation project)
    {
        var _project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (_project is null) return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _project.UserId.ToString()) return Results.Forbid();

        _project.Copy(project);

        await _context.SaveChangesAsync();
        return Results.Created($"/calendar/{_project.Id}", _project);
    }

    [Route("member")]
    [HttpPost("member/{id}")]
    public async Task<IResult> AddMembers(int id, MemberInformation[] members)
    {
        var _project = await _context.Projects
            .Include(e => e.Members).FirstOrDefaultAsync(e => e.Id == id);
        if (_project == null) return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _project.UserId.ToString()) return Results.Forbid();

        foreach (var member in members)
        {
            var studentNum = await _context.StudentNums.FindAsync(member.StudentNumber);
            if (studentNum == null)
            {
                _context.RevertChanges();
                return Results.NotFound();
            }

            if (_project.Members.FirstOrDefault
                (
                    m => m.StudentNumber == member.StudentNumber && m.Role == member.Role
                ) != null)
            {
                _context.RevertChanges();
                return Results.Conflict();
            }

            var newMember = new Member
            {
                ProjectId = id,
                StudentNumber = member.StudentNumber,
                Index = member.Index,
                Role = member.Role,
                Description = member.Description
            };

            await _context.Members.AddAsync(newMember);
        }

        await _context.SaveChangesAsync();
        return Results.Created<Project>($"/project/member/{id}", _project);
    }

    [Route("member")]
    [HttpPut("member/{id}")]
    public async Task<IResult> UpdateMember(int id, MemberInformation member)
    {
        var _member = await _context.Members.FindAsync(id);
        if (_member == null) return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _member.ProjectId.ToString()) return Results.Forbid();

        _member.Description = member.Description;
        _member.Index = member.Index;

        await _context.SaveChangesAsync();
        return Results.Created($"/project/member/{id}", _member);
    }

    [Route("member")]
    [HttpDelete("member/{id}")]
    public async Task<IResult> DeleteMember(int id)
    {
        var _member = await _context.Members.FindAsync(id);
        if (_member == null) return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _member.ProjectId.ToString()) return Results.Forbid();

        _context.Members.Remove(_member);
        await _context.SaveChangesAsync();
        return Results.Ok(_member);
    }
}