using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using ZDrive.Controllers;
using ZDrive.Data;
using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive_Test;

public class ProjectControllerTest
{
    private TestDbContextCreater testDbCreater = null!;

    [Test]
    public async Task Create_ExistProjectName_ReturnsConflictStatusCode()
    {
        // Assert
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Id = 2,
            Name = "Baba Is You"
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.Create(project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Conflict)));
    }

    [Test]
    public async Task Create_ValidProject_ShouldBeAdded()
    {
        // Assert
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Id = 2,
            Name = "TEST"
        };
        var cnt = context.Projects.Count();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Create(project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<Project>)));
        Assert.That(context.Projects.Count(), Is.EqualTo(cnt + 1));
    }

    [Test]
    public async Task Read_NonProjectId_ReturnsNotFoundCode()
    {
        // Assert
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Read(17);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task Read_ExistProjectId_ReturnsThatProject()
    {
        // Assert
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Read(1);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Ok<Project>)));
        var value = (ret as Microsoft.AspNetCore.Http.HttpResults.Ok<Project>)?.Value;
        Assert.That(value, Is.Not.Null);
        Assert.That(value?.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task ReadAllProject_ReturnsAllProjects()
    {
        // Assert
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.ReadAllProject();

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Ok<List<Project>>)));
        var value = (ret as Microsoft.AspNetCore.Http.HttpResults.Ok<List<Project>>)?.Value;
        Assert.That(value, Is.Not.Null);
        Assert.That(value?.Count, Is.EqualTo(context.Projects.Count()));
    }

    [Test]
    public async Task Update_NonProjectId_ReturnsNotFoundCode()
    {
        // Assert
        var project = new Project
        {
            Name = "Baba Is You"
        };
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Update(2, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task Update_ExistProjectId_ShouldUpdateProject()
    {
        // Assert
        var project = new Project
        {
            Name = "Baba Is You",
            Description = "Baba Is You!"
        };
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Update(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<Project>)));
        Assert.That(context.Projects.First(u => u.Id == 1)?.Description, Is.EqualTo("Baba Is You!"));
    }

    [Test]
    public async Task Delete_NonProjectId_ReturnsNotFoundCode()
    {
        // Assert
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Delete(2);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task Delete_ExistProjectId_ShouldBeRemoved()
    {
        // Assert
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Delete(1);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Ok<Project>)));
        Assert.That(context.Projects.FirstOrDefault(p => p.Id == 1), Is.Null);
    }

    [SetUp]
    public void SetUp()
    {
        testDbCreater = new TestDbContextCreater();
        testDbCreater.Setup(c => 
        {
            c.Users.AddRange(fakeUserList);
            c.StudentNums.AddRange(fakeStdNumList);
            c.Projects.AddRange(fakeProjectList);
            c.SaveChanges();
        });
    }

    [TearDown]
    public void TearDown()
    {
        testDbCreater.Dispose();
    }

    private ProjectController CreateController(ZDriveDbContext context)
        => new ProjectController(context);

    private User[] fakeUserList = new User[]
    {
        new User
        {
            Id = 1,
            Name = "Jun",
            StudentNumber = "2021320003",
            PasswordHash = "Vut+mCn8wSAlGPmOSA+qfXA6b7/wdpQL2jjFYUcIEMU=", // password: password
            Salt = "realtest",
            IsVerified = false
        },
        new User
        {
            Id = 2,
            Name = "Chaenamul",
            StudentNumber = "2020320124",
            PasswordHash = "iQPdQPkVlcPYd2H4LCJC1vOEohqnSLgdmOjfFsOEOjk=", // password: drowssap
            Salt = "realtest",
            IsVerified = true
        }
    };

    private StudentNum[] fakeStdNumList = new StudentNum[]
    {
        new StudentNum
        {
            StudentNumber = "2021320003",
            Name = "Jun"
        },
        new StudentNum
        {
            StudentNumber = "2020320124",
            Name = "Chaenamul"
        }
    };

    private Project[] fakeProjectList = new Project[]
    {
        new Project
        {
            Id = 1,
            Name = "Baba Is You",
            Description = "Test game project",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30)
        }
    };
}