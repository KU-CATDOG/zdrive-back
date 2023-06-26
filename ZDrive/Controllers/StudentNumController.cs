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
[Route("num")]
public class StudentNumController : ControllerBase
{
    private readonly ZDriveDbContext _context;

    public StudentNumController(ZDriveDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IResult> Create(StudentNumInfo num)
    {
        if ((await _context.StudentNums.FindAsync(num.StudentNumber)) != null)
            return Results.Conflict();

        var newNum = new StudentNum()
        {
            StudentNumber = num.StudentNumber,
            Name = num.Name
        };

        await _context.StudentNums.AddAsync(newNum);
        await _context.SaveChangesAsync();

        return Results.Created($"/num/{newNum.StudentNumber}", newNum);
    }

    [HttpGet("{num}")]
    public async Task<IResult> Read(string num)
    {
        var stdNum = await _context.StudentNums.FindAsync(num);
        return stdNum == null ? Results.NotFound() : Results.Ok(stdNum);
    }
}
