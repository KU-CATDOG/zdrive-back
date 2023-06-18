using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Extensions;
using ZDrive.Models;
using ZDrive.Services;

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

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            Genre = project.Genre,
            Engine = project.Engine,
            FileSrc = project.FileSrc,
            UserId = int.Parse(sid)
        };

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
    public async Task<IResult> Read(int id)
    {
        var project = await _context.Projects.Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);
        return project == null ? Results.NotFound() : Results.Ok(project);
    }

    [Route("list")]
    [HttpGet]
    public async Task<IResult> ReadAllProject()
    {
        var projects = await _context.Projects.ToListAsync();
        return Results.Ok(projects);
    }

    [HttpPut("{id}")]
    public async Task<IResult> Update(int id, ProjectInformation project)
    {
        var _project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (_project is null) return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _project.UserId.ToString()) return Results.Forbid();

        _project.Name = project.Name;
        _project.Description = project.Description;
        _project.StartDate = project.StartDate;
        _project.EndDate = project.EndDate;
        _project.Status = project.Status;
        _project.Genre = project.Genre;
        _project.Engine = project.Engine;
        _project.FileSrc = project.FileSrc;

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