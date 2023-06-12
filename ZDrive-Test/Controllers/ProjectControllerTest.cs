using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Id = 2,
            Name = "Baba Is You",
            UserId = 1
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
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Id = 2,
            Name = "TEST",
            UserId = 1
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
        // Arrange
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
        // Arrange
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
        // Arrange
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
        // Arrange
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
        // Arrange
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
    public async Task AddMembers_NonProjectId_ReturnsNotFoundCode()
    {
        // Arrange
        var project = new Project
        {
            Name = "Baba Is You"
        };
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(2, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task AddMembers_NotExistStudentNum_ReturnsNotFoundStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Members = new List<Member>
            {
                new Member
                {
                    StudentNumber = "2021320006",
                    Role = Role.Programmer
                }
            }
        };
        var cnt = context.Members.Count();
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
        Assert.That(context.Members.Count(), Is.EqualTo(cnt));
    }

    [Test]
    public async Task AddMembers_ProjectIdInMember_ShouldBeEqualToProjectId()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Members = new List<Member>
            {
                new Member
                {
                    Id = 2,
                    StudentNumber = "2021320003",
                    Role = Role.GameDesigner
                }
            }
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, project);

        // Assert
        var member = await context.Members.FindAsync(2);
        Assert.That(member, Is.Not.Null);
        Assert.That(member?.ProjectId, Is.EqualTo(1));
    }

    [Test]
    public async Task AddMembers_ValidMembers_ShouldBeAdded()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Members = new List<Member>
            {
                new Member
                {
                    Id = 2,
                    StudentNumber = "2021320003",
                    Role = Role.GameDesigner
                }
            }
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<Project>)));
        var member = await context.Members.FindAsync(2);
        Assert.That(member, Is.Not.Null);
    }

    [Test]
    public async Task AddMembers_ExistMemberWithRole_ReturnsConflictStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Members = new List<Member>
            {
                new Member
                {
                    StudentNumber = "2021320003",
                    Role = Role.Programmer
                }
            }
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Conflict)));
    }

    [Test]
    public async Task AddMembers_UserThatIsNotOwner_ReturnsForbidStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var project = new Project
        {
            Members = new List<Member>
            {
                new Member
                {
                    StudentNumber = "2021320003",
                    Role = Role.GameDesigner
                }
            }
        };
        var controller = CreateController(context, 2);

        // Act
        var ret = await controller.AddMembers(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
    }

    [Test]
    public async Task Delete_NonProjectId_ReturnsNotFoundCode()
    {
        // Arrange
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
        // Arrange
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
            c.Members.AddRange(fakeMemberList);
            c.SaveChanges();
        });
    }

    [TearDown]
    public void TearDown()
    {
        testDbCreater.Dispose();
    }

    private ProjectController CreateController(ZDriveDbContext context, int userId = 1)
    {
        var controller = new ProjectController(context);

        var claims = new[] { 
            new Claim(ClaimTypes.Sid, userId.ToString()),
            new Claim(ClaimTypes.Role, Authority.User.ToString()) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));

        var contextMock = new Mock<HttpContext>();
        contextMock.SetupProperty(ctx => ctx.User, principal);

        var controllerContextMock = new ControllerContext();
        controllerContextMock.HttpContext = contextMock.Object;

        controller.ControllerContext = controllerContextMock;
        return controller;
    }

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
            EndDate = DateTime.Now.AddDays(30),
            UserId = 1
        }
    };

    private Member[] fakeMemberList = new Member[]
    {
        new Member
        {
            Id = 1,
            ProjectId = 1,
            StudentNumber = "2021320003",
            Role = Role.Programmer,
            Description = "Test"
        }
    };
}