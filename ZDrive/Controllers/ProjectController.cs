using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
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

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };

        await _context.Projects.AddAsync(newProject);
        await _context.SaveChangesAsync();

        return Results.Created($"/calendar/{newProject.Id}", project);
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

        if (_project is null) return TypedResults.NotFound();

        // Fix me: 프로젝트 Owner만 프로젝트 삭제 편집 가능하게 해야함

        _project.Name = project.Name;
        _project.Description = project.Description;
        _project.StartDate = project.StartDate;
        _project.EndDate = project.EndDate;

        await _context.SaveChangesAsync();

        return TypedResults.Created($"/calendar/{_project.Id}", _project);
    }
}