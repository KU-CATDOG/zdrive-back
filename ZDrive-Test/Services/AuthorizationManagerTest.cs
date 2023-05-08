using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Moq;
using ZDrive.Controllers;
using ZDrive.Services;

namespace ZDrive_Test;

public class AuthorizationManagerTest
{
    private int userId;
    private Mock<ISessionStorage> mockSesson = new Mock<ISessionStorage>();
    private Guid ssid;

    [Test]
    public void CheckSession_ValidCookie_ReturnsUserId()
    {
        // Arrange
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizationManager(session);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.Setup(foo => foo.Cookies["sessionId"]).Returns(ssid.ToString());
        HttpRequest req = mockHttpRequest.Object;

        // Act
        var ret = controller.CheckSession(req, out var id);

        // Assert
        Assert.AreEqual(Results.Ok(), ret);
        Assert.AreEqual(userId, id);
    }

    [Test]
    public void CheckSession_EmptyCookie_ReturnsUnauthorizedStatusCode()
    {
        // Arrange
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizationManager(session);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.Setup(foo => foo.Cookies["sessionId"]).Returns(() => null);
        HttpRequest req = mockHttpRequest.Object;

        // Act
        var ret = controller.CheckSession(req, out var id);

        // Assert
        Assert.AreEqual(Results.Unauthorized(), ret);
        Assert.AreEqual(default(int), id);
    }

    [Test]
    public void CheckSession_InvalidCookie_ReturnsBadRequestStatusCode()
    {
        // Arrange
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizationManager(session);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.Setup(foo => foo.Cookies["sessionId"]).Returns(() => "Invalid Cookie");
        HttpRequest req = mockHttpRequest.Object;

        // Act
        var ret = controller.CheckSession(req, out var id);

        // Assert
        Assert.AreEqual(Results.BadRequest(), ret);
        Assert.AreEqual(default(int), id);
    }

    [Test]
    public void CheckSession_NonSSID_ReturnsUnauthorizedStatusCode()
    {
        // Arrange
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizationManager(session);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.Setup(foo => foo.Cookies["sessionId"]).Returns(Guid.NewGuid().ToString());
        HttpRequest req = mockHttpRequest.Object;

        // Act
        var ret = controller.CheckSession(req, out var id);

        // Assert
        Assert.AreEqual(Results.Unauthorized(), ret);
        Assert.AreEqual(default(int), id);
    }

    [Test]
    public void CheckSession_InsufficientAuthority_ReturnsForbidStatusCode()
    {
        // Arrange
        var outSession = new Session(userId, 0, DateTime.Now);
        mockSesson.Setup(foo => foo.AddSession(userId, 0, out ssid)).Returns(true);
        mockSesson.Setup(foo => foo.TryGetUser(ssid, out outSession)).Returns(true);

        ISessionStorage session = mockSesson.Object;
        session.AddSession(userId, 0, out var outSsid);

        var controller = new AuthorizationManager(session);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.Setup(foo => foo.Cookies["sessionId"]).Returns(outSsid.ToString());
        HttpRequest req = mockHttpRequest.Object;

        // Act
        var ret = controller.CheckSession(req, ZDrive.Models.Authority.Administer, out var id);

        // Assert
        Assert.That(ret, Is.TypeOf(typeof(Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult)));
        Assert.AreEqual(default(int), id);
    }

    [SetUp]
    public void SetUp()
    {
        userId = 1;
        var outUserId = 1;
        var outSession = new Session(outUserId, DateTime.Now);
        ssid = Guid.NewGuid();
        mockSesson.Setup(foo => foo.AddSession(userId, out ssid)).Returns(true);
        mockSesson.Setup(foo => foo.TryGetUser(ssid, out outSession)).Returns(true);
    }
}
