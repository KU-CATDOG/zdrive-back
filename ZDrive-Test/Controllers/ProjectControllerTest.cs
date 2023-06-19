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
        var project = new ProjectInformation
        {
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
        var project = new TestDataBuilder<ProjectInformation>()
            .Randomize()
            .Build();
        var cnt = context.Projects.Count();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Create(project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<Project>)));
        Assert.That(context.Projects.Find(3), Is.Not.Null);
        Assert.That(context.Projects.Find(3)?.Name, Is.EqualTo(project.Name));
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
    public async Task Read_UnauthenticatedUserReadPrivateProject_ReturnsNotFoundStatus()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateUnauthenticatedController(context);

        // Act
        var ret = await controller.Read(1);

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
        Assert.That(value?.Members.FirstOrDefault(m => m.Id == 1), Is.Not.Null);
    }

    [Test]
    public async Task ReadAllProject_AuthenticatedUser_ReturnsAllProjects()
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
    public async Task ReadAllProject_UnauthenticatedUser_ReturnsPublicProjects()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateUnauthenticatedController(context);

        // Act
        var ret = await controller.ReadAllProject();

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Ok<List<Project>>)));
        var value = (ret as Microsoft.AspNetCore.Http.HttpResults.Ok<List<Project>>)?.Value;
        Assert.That(value, Is.Not.Null);
        Assert.That(value?.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task Update_NonProjectId_ReturnsNotFoundCode()
    {
        // Arrange
        var project = new ProjectInformation();
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Update(3, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task Update_ExistProjectId_ShouldUpdateProject()
    {
        // Arrange
        var project = new TestDataBuilder<ProjectInformation>()
            .Randomize()
            .Build();
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Update(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<Project>)));
        Assert.That(context.Projects.First(u => u.Id == 1)?.Description, Is.EqualTo(project.Description));
    }

    [Test]
    public async Task Update_UserThatIsNotOwner_ReturnsForbidStatusCode()
    {
        // Arrange
        var project = new ProjectInformation
        {
            Name = "Baba Is You",
            Description = "Baba Is You!"
        };
        using var context = testDbCreater.Create();
        var controller = CreateController(context, 2);

        // Act
        var ret = await controller.Update(1, project);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
    }

    [Test]
    public async Task AddMembers_NonProjectId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var members = new MemberInformation[] { };
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(3, members);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task AddMembers_NotExistStudentNum_ReturnsNotFoundStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var members = new MemberInformation[]
        {
            new MemberInformation
            {
                StudentNumber = "2021320006",
                Role = Role.Programmer
            }
        };
        var cnt = context.Members.Count();
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, members);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
        Assert.That(context.Members.Count(), Is.EqualTo(cnt));
    }

    [Test]
    public async Task AddMembers_ProjectIdInMember_ShouldBeEqualToProjectId()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var members = new MemberInformation[]
        {
            new TestDataBuilder<MemberInformation>()
            .Randomize()
            .Setup(m => m.StudentNumber = "2020320124")
            .Build()
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, members);

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
        var members = new MemberInformation[]
        {
            new TestDataBuilder<MemberInformation>()
            .Randomize()
            .Setup(m => m.StudentNumber = "2021320003")
            .Setup(m => m.Role = Role.GameDesigner)
            .Build()
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, members);

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
        var members = new MemberInformation[]
        {
            new MemberInformation
            {
                StudentNumber = "2021320003",
                Role = Role.Programmer
            }
        };
        var controller = CreateController(context);

        // Act
        var ret = await controller.AddMembers(1, members);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Conflict)));
    }

    [Test]
    public async Task AddMembers_UserThatIsNotOwner_ReturnsForbidStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var members = new MemberInformation[] { };
        var controller = CreateController(context, 2);

        // Act
        var ret = await controller.AddMembers(1, members);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
    }

    [Test]
    public async Task UpdateMember_NonMemberId_ReturnsNotFountStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var member = new MemberInformation();
        var controller = CreateController(context);

        // Act
        var ret = await controller.UpdateMember(2, member);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task UpdateMember_UserThatIsNotOwner_ReturnsForbidStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var member = new MemberInformation();
        var controller = CreateController(context, 2);

        // Act
        var ret = await controller.UpdateMember(1, member);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
    }

    [Test]
    public async Task UpdateMember_ExistMemberId_ShouldUpdateMember()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var member = new TestDataBuilder<MemberInformation>().Randomize().Build();
        var controller = CreateController(context);

        // Act
        var ret = await controller.UpdateMember(1, member);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<Member>)));
        Assert.That(context.Members.First(u => u.Id == 1)?.Description, Is.EqualTo(member.Description));
    }

    [Test]
    public async Task DeleteMember_NonMemberId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.DeleteMember(2);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.NotFound)));
    }

    [Test]
    public async Task DeleteMember_UserThatIsNotOwner_ReturnsForbidStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateController(context, 2);

        // Act
        var ret = await controller.DeleteMember(1);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
    }

    [Test]
    public async Task DeleteMember_ExistMemberId_ShouldBeRemoved()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.DeleteMember(1);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Ok<Member>)));
        Assert.That(context.Members.FirstOrDefault(p => p.Id == 1), Is.Null);
    }

    [Test]
    public async Task Delete_NonProjectId_ReturnsNotFoundCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateController(context);

        // Act
        var ret = await controller.Delete(3);

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

    [Test]
    public async Task Delete_UserThatIsNotOwner_ReturnsForbidStatusCode()
    {
        // Arrange
        using var context = testDbCreater.Create();
        var controller = CreateController(context, 2);

        // Act
        var ret = await controller.Delete(1);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
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

    private ProjectController CreateUnauthenticatedController(ZDriveDbContext context)
    {
        var controller = new ProjectController(context);
        var principal = new ClaimsPrincipal();

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
            UserId = 1,
            Visibility = Visibility.Private
        },
        new Project
        {
            Id = 2,
            Name = "You Is Baba",
            Description = "Test game project",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            UserId = 1,
            Visibility = Visibility.Public
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