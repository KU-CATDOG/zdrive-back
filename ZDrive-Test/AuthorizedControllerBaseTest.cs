using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Moq;
using ZDrive.Controllers;
using ZDrive.Services;

namespace ZDrive_Test;

public class AuthorizedControllerBaseTest
{
    [Test]
    public void CheckSession_ValidCookie_ReturnsUserId()
    {
        // Arrange
        var userId = 1;
        var outUserId = 1;
        var ssid = Guid.NewGuid();
        var mockSesson = new Mock<ISessionStorage>();
        mockSesson.Setup(foo => foo.AddSession(userId, out ssid)).Returns(true);
        mockSesson.Setup(foo => foo.TryGetUser(ssid, out outUserId)).Returns(true);
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizedControllerBase(session);
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
        var userId = 1;
        var outUserId = 1;
        var ssid = Guid.NewGuid();
        var mockSesson = new Mock<ISessionStorage>();
        mockSesson.Setup(foo => foo.AddSession(userId, out ssid)).Returns(true);
        mockSesson.Setup(foo => foo.TryGetUser(ssid, out outUserId)).Returns(true);
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizedControllerBase(session);
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
        var userId = 1;
        var outUserId = 1;
        var ssid = Guid.NewGuid();
        var mockSesson = new Mock<ISessionStorage>();
        mockSesson.Setup(foo => foo.AddSession(userId, out ssid)).Returns(true);
        mockSesson.Setup(foo => foo.TryGetUser(ssid, out outUserId)).Returns(true);
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizedControllerBase(session);
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
        var userId = 1;
        var outUserId = 1;
        var ssid = Guid.NewGuid();
        var mockSesson = new Mock<ISessionStorage>();
        mockSesson.Setup(foo => foo.AddSession(userId, out ssid)).Returns(true);
        mockSesson.Setup(foo => foo.TryGetUser(ssid, out outUserId)).Returns(true);
        ISessionStorage session = mockSesson.Object;

        session.AddSession(userId, out var outSsid);

        var controller = new AuthorizedControllerBase(session);
        var mockHttpRequest = new Mock<HttpRequest>();
        mockHttpRequest.Setup(foo => foo.Cookies["sessionId"]).Returns(Guid.NewGuid().ToString());
        HttpRequest req = mockHttpRequest.Object;

        // Act
        var ret = controller.CheckSession(req, out var id);

        // Assert
        Assert.AreEqual(Results.Unauthorized(), ret);
        Assert.AreEqual(default(int), id);
    }
}
