using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Extensions;
using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive.Controllers;

public class ProjectController : ControllerBase
{
    private readonly ZDriveDbContext _context;

    public ProjectController(ZDriveDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IResult> Create(Project project)
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
            UserId = int.Parse(sid)
        };

        await _context.Projects.AddAsync(newProject);
        await _context.SaveChangesAsync();

        return Results.Created($"/project/{newProject.Id}", project);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(int id)
    {
        var _project = await _context.Projects.FindAsync(id);
        if (_project == null)
            return Results.NotFound();

        // Fix me: 프로젝트 Owner만 프로젝트 삭제 편집 가능하게 해야함

        _context.Projects.Remove(_project);
        await _context.SaveChangesAsync();
        return Results.Ok(_project);
    }

    [HttpGet("{id}")]
    public async Task<IResult> Read(int index)
    {
        var project = await _context.Projects.FindAsync(index);
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
    public async Task<IResult> Update(int id, Project project)
    {
        var _project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (_project is null) return Results.NotFound();

        // Fix me: 프로젝트 Owner만 프로젝트 삭제 편집 가능하게 해야함

        _project.Name = project.Name;
        _project.Description = project.Description;
        _project.StartDate = project.StartDate;
        _project.EndDate = project.EndDate;

        await _context.SaveChangesAsync();

        return Results.Created($"/calendar/{_project.Id}", _project);
    }

    [Route("member")]
    [HttpPost("{id}")]
    public async Task<IResult> AddMembers(int id, Project project)
    {
        var _project = await _context.Projects
            .Include(e => e.Members).FirstOrDefaultAsync(e => e.Id == id);
        if (_project == null) return Results.NotFound();

        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();
        if (sid != _project.UserId.ToString()) return Results.Forbid();

        foreach (var member in project.Members)
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
        return Results.Created<Project>($"/project/{id}", _project);
    }
}