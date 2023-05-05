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
    private Mock<ZDriveDbContext> mockZDriveDbContext = new Mock<ZDriveDbContext>();
    private Mock<IAuthorizationManager> mockAuthorizationManager = new Mock<IAuthorizationManager>();
    private Mock<ISessionStorage> mockSessionStorage = new Mock<ISessionStorage>();

    private AuthController CreateAuthController()
    {
        var controller = new AuthController(mockZDriveDbContext.Object, mockAuthorizationManager.Object, mockSessionStorage.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
        return controller;
    }

    [Test]
    public void Login_ValidNameAndPassword_ReturnsOkStatusCode()
    {
        // Arrange
        var dict = new Dictionary<Guid, Session>();
        var ssid = Guid.NewGuid();

        mockSessionStorage.Setup(x => x.Session).Returns(dict.AsReadOnly());
        mockSessionStorage.Setup(x => x.AddSession(2, out ssid))
            .Returns(() => { dict[ssid] = new Session(2, DateTime.Now); return true; });

        // Act
        var controller = CreateAuthController();
        var user = new User
        {
            Name = "Chaenamul",
            PasswordHash = "drowssap"
        };

        var ret = controller.Login(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.Ok()));
        Assert.That(mockSessionStorage.Object.Session.Count, Is.EqualTo(1));
    }

    [Test]
    public void Login_IncorrectPassword_ReturnsUnauthorizedStatusCode()
    {
        // Act
        var controller = CreateAuthController();
        var user = new User
        {
            Name = "Chaenamul",
            PasswordHash = "naifoewfho23hpqiofhoi23fh"
        };

        var ret = controller.Login(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.Unauthorized()));
    }

    [Test]
    public void Login_NonExistUser_ReturnsNotFoundStatusCode()
    {
        // Act
        var controller = CreateAuthController();
        var user = new User
        {
            Name = "Merseong",
            PasswordHash = "naifoewfho23hpqiofhoi23fh"
        };

        var ret = controller.Login(user);

        // Assert
        Assert.That(ret, Is.EqualTo(Results.NotFound()));
    }

    [Test]
    public void Login_NotVerifiedUser_ReturnsForbidStatusCode()
    {
        // Act
        var controller = CreateAuthController();
        var user = new User
        {
            Name = "Tom",
            PasswordHash = "naifoewfho23hpqiofhoi23fh"
        };

        var ret = controller.Login(user);

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

        // Act
        var controller = CreateAuthController();
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

        // Act
        var controller = CreateAuthController();
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

        // Act
        var controller = CreateAuthController();
        var ret = controller.Logout();

        // Assert
        Assert.That(ret, Is.EqualTo(Results.BadRequest()));
        Assert.That(dict.ContainsKey(ssid), Is.True);
    }

    [SetUp]
    public void SetUp()
    {
        var id = 1;
        var mockCookie = new Mock<IResponseCookies>();
        mockHttpResponse.Setup(foo => foo.Cookies).Returns(mockCookie.Object);
        mockHttpContext.Setup(foo => foo.Response).Returns(mockHttpResponse.Object);
        mockHttpContext.Setup(foo => foo.Request).Returns(mockHttpRequest.Object);
        mockZDriveDbContext.Setup(foo => foo.Users)
            .ReturnsDbSet(GetFakeUserList());
        mockAuthorizationManager.Setup(foo => foo.CheckSession(mockHttpRequest.Object, out id))
            .Returns(Results.Ok());
    }

    private List<User> GetFakeUserList()
    {
        return new List<User>()
        {
            new User
            {
                Id = 1,
                Name = "Tom",
                StudentNumber = "2021320006",
                PasswordHash = "Vut+mCn8wSAlGPmOSA+qfXA6b7/wdpQL2jjFYUcIEMU=",
                Salt = "realtest",
                IsVerified = false
            },
            new User
            {
                Id = 2,
                Name = "Chaenamul",
                StudentNumber = "2020320124",
                PasswordHash = "iQPdQPkVlcPYd2H4LCJC1vOEohqnSLgdmOjfFsOEOjk=",
                Salt = "realtest",
                IsVerified = true
            }
        };
    }
}