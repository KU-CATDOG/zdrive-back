using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using ZDrive.Controllers;
using ZDrive.Data;
using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive_Test;

public class AuthControllerTest
{
    private Mock<HttpRequest> mockHttpRequest = new Mock<HttpRequest>();
    private Mock<HttpResponse> mockHttpResponse = new Mock<HttpResponse>();
    private Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
    private TestDbContextCreater testDbContextCreater = null!;
    private Mock<IAuthorizationManager> mockAuthorizationManager = new Mock<IAuthorizationManager>();
    private Mock<ISessionStorage> mockSessionStorage = new Mock<ISessionStorage>();

    private AuthController CreateAuthController(ZDriveDbContext context)
    {
        var controller = new AuthController(context, mockAuthorizationManager.Object, mockSessionStorage.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
        return controller;
    }

    [Test]
    public async Task Login_ValidStudentNumAndPassword_ReturnsOkStatusCodeAsync()
    {
        // Arrange
        var dict = new Dictionary<Guid, Session>();
        var ssid = Guid.NewGuid();

        mockSessionStorage.Setup(x => x.Session).Returns(dict.AsReadOnly());
        mockSessionStorage.Setup(x => x.AddSession(2, out ssid))
            .Returns(() => { dict[ssid] = new Session(2, DateTime.Now); return true; });

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var user = new Login
        {
            StudentNumber = "2020320124",
            Password = "drowssap"
        };

        var ret = await controller.Login(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.Ok()));
        Assert.That(mockSessionStorage.Object.Session.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task Login_IncorrectPassword_ReturnsNotFoundStatusCodeAsync()
    {
        // Assert
        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var user = new Login
        {
            StudentNumber = "2020320124",
            Password = "naifoewfho23hpqiofhoi23fh"
        };

        var ret = await controller.Login(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.NotFound()));
    }

    [Test]
    public async Task Login_NonExistStudentNum_ReturnsNotFoundStatusCodeAsync()
    {
        // Assert
        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var user = new Login
        {
            StudentNumber = "2021320108",
            Password = "naifoewfho23hpqiofhoi23fh"
        };

        var ret = await controller.Login(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.NotFound()));
    }

    [Test]
    public async Task Login_NotVerifiedUser_ReturnsForbidStatusCodeAsync()
    {
        // Assert
        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var user = new Login
        {
            StudentNumber = "2021320003",
            Password = "password"
        };

        var ret = await controller.Login(user);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
    }

    [Test]
    public void Logout_ExistSSID_ShouldBeRemovedInSessionStorage()
    {
        // Arrange
        var dict = new Dictionary<Guid, Session>();
        var ssid = Guid.NewGuid();

        mockSessionStorage.Setup(x => x.RemoveUser(ssid))
            .Callback(() => dict.Remove(ssid));
        mockHttpRequest.Setup(x => x.Cookies["sessionId"]).Returns(ssid.ToString());

        dict[ssid] = new Session(2, DateTime.Now);
        
        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = controller.Logout();

        // Assert
        Assert.That(ret, Is.EqualTo(Results.Ok()));
        Assert.That(dict.ContainsKey(ssid), Is.False);
    }

    [Test]
    public void Logout_NonExistSSID_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var dict = new Dictionary<Guid, Session>();
        var ssid = Guid.NewGuid();
        var newSsid = Guid.NewGuid();

        mockSessionStorage.Setup(x => x.RemoveUser(newSsid))
            .Callback(() => throw new KeyNotFoundException());
        mockHttpRequest.Setup(x => x.Cookies["sessionId"]).Returns(newSsid.ToString());

        dict[ssid] = new Session(2, DateTime.Now);

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = controller.Logout();

        // Assert
        Assert.That(ret, Is.EqualTo(Results.NotFound()));
        Assert.That(dict.ContainsKey(ssid), Is.True);
    }

    [Test]
    public void Logout_InvalidSSID_ReturnsBadRequestStatusCode()
    {
        // Arrange
        var dict = new Dictionary<Guid, Session>();
        var ssid = Guid.NewGuid();

        mockSessionStorage.Setup(x => x.RemoveUser(ssid))
            .Callback(() => dict.Remove(ssid));
        mockHttpRequest.Setup(x => x.Cookies["sessionId"]).Returns("InvalidSSID");

        dict[ssid] = new Session(2, DateTime.Now);

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = controller.Logout();

        // Assert
        Assert.That(ret, Is.EqualTo(Results.BadRequest()));
        Assert.That(dict.ContainsKey(ssid), Is.True);
    }

    [Test]
    public async Task Register_ValidUserInformation_ShouldBeAddedInDB()
    {
        // Arrange
        var studentNumber = "2021320006";
        var registration = new Registration
        {
            Name = "Minjong",
            StudentNumber = studentNumber,
            Password = "passdrow",
        };

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = await controller.Register(registration);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Created<User>)));
        Assert.That(context.StudentNums.FirstOrDefault(x => x.StudentNumber == studentNumber), Is.Not.Null);
        Assert.That(context.Users.FirstOrDefault(x => x.StudentNumber == studentNumber), Is.Not.Null);
    }

    [Test]
    public async Task Register_ExistStudentNumber_ReturnConflictStatusCode()
    {
        // Arrange
        var name = "Merseong";
        var registration = new Registration
        {
            Name = name,
            StudentNumber = "2021320003",
            Password = "passdrow",
        };

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = await controller.Register(registration);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.Conflict)));
    }

    [Test]
    public async Task Remove_ValidStudentNumAndPassword_ShouldRemoveUserInDBAndSession()
    {
        // Arrange
        var ssid = Guid.NewGuid();
        var session = new Dictionary<Guid, Session> { { ssid, new Session(2, DateTime.Now) } };

        mockSessionStorage.Setup(x => x.RemoveUser(2))
            .Callback(() => { session.Remove(ssid); });

        var user = new Login
        {
            StudentNumber = "2020320124",
            Password = "drowssap"
        };

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = await controller.Remove(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.Ok()));
        Assert.That(context.Users.FirstOrDefault(x => x.StudentNumber == user.StudentNumber), Is.Null);
        Assert.That(session.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task Remove_NonStudentNum_ReturnsNotFoundStatusCodeAsync()
    {
        // Assert
        var user = new Login
        {
            StudentNumber = "2023235304",
            Password = "drowssap"
        };

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = await controller.Remove(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.NotFound()));
    }

    [Test]
    public async Task Remove_InvalidPassword_ReturnsNotFoundStatusCodeAsync()
    {
        // Assert
        var user = new Login
        {
            StudentNumber = "2020320124",
            Password = "asdasd"
        };

        using var context = testDbContextCreater.Create();
        var controller = CreateAuthController(context);

        // Act
        var ret = await controller.Remove(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.NotFound()));
    }

    // Todo: 계정 비밀번호 복구하는거 만들어야함
    // [Test]
    // public void Recover_ValidStudentNum_ShouldResetPasswordToDefault()
    // {
    //     throw new NotImplementedException();
    // }

    // [Test]
    // public void Recover_NonStudentNum_ReturnsNotFoundStatusCode()
    // {
    //     throw new NotImplementedException();
    // }

    // [Test]
    // public void Recover_InsufficientAuthority_ReturnsForbidStatusCode()
    // {
    //     throw new NotImplementedException();
    // }

    [SetUp]
    public void SetUp()
    {
        var id = 1;
        var mockCookie = new Mock<IResponseCookies>();
        mockHttpResponse.Setup(foo => foo.Cookies).Returns(mockCookie.Object);
        mockHttpContext.Setup(foo => foo.Response).Returns(mockHttpResponse.Object);
        mockHttpContext.Setup(foo => foo.Request).Returns(mockHttpRequest.Object);

        testDbContextCreater = new TestDbContextCreater();
        testDbContextCreater.Setup(c => 
        {
            c.Users.AddRange(fakeUserList);
            c.StudentNums.AddRange(fakeStdNumList);
            c.SaveChanges();
        });

        mockAuthorizationManager.Setup(foo => foo.CheckSession(mockHttpRequest.Object, out id))
            .Returns(Results.Ok());
    }

    [TearDown]
    public void TearDown()
    {
        testDbContextCreater.Dispose();
    }

    private User[] fakeUserList = new User[]
    {
        new User
        {
            Id = 1,
            Name = "Jun",
            StudentNumber = "2021320003",
            PasswordHash = "Vut+mCn8wSAlGPmOSA+qfXA6b7/wdpQL2jjFYUcIEMU=",
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
}