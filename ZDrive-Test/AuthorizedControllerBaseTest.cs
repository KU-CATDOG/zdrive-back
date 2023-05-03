using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using ZDrive.Controllers;
using ZDrive.Services;

namespace ZDrive_Test;

public class AuthorizedControllerBaseTest
{
    private HttpRequest CreateHttpRequest(string? ssid)
        => new TestHttpRequest(new TestCookieColletion(ssid));

    [Test]
    public void CheckSession_ValidCookie_ReturnsUserId()
    {
        // Arrange
        var userId = 1;
        ISessionStorage session = new TestSessionStorage();
        session.AddSession(userId, out var ssid);

        var controller = new AuthorizedControllerBase(session);
        HttpRequest req = CreateHttpRequest(ssid.ToString());

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
        ISessionStorage session = new TestSessionStorage();
        session.AddSession(userId, out var ssid);

        var controller = new AuthorizedControllerBase(session);
        HttpRequest req = CreateHttpRequest(null);

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
        ISessionStorage session = new TestSessionStorage();
        session.AddSession(userId, out var ssid);

        var controller = new AuthorizedControllerBase(session);
        HttpRequest req = CreateHttpRequest("Invalid_SSID");

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
        ISessionStorage session = new TestSessionStorage();
        session.AddSession(userId, out var ssid);

        var controller = new AuthorizedControllerBase(session);
        HttpRequest req = CreateHttpRequest(Guid.NewGuid().ToString());

        // Act
        var ret = controller.CheckSession(req, out var id);

        // Assert
        Assert.AreEqual(Results.Unauthorized(), ret);
        Assert.AreEqual(default(int), id);
    }

    private class TestSessionStorage : ISessionStorage
    {
        private Dictionary<Guid, Session> _session = new Dictionary<Guid, Session>();
        public ReadOnlyDictionary<Guid, Session> Session => _session.AsReadOnly();

        public bool AddSession(int userId, out Guid ssid)
        {
            Guid guid = Guid.NewGuid();
            ssid = guid;
            _session[guid] = new Session(userId, DateTime.Now);
            return true;
        }

        public bool AddSession(int userId, DateTime dateTime, out Guid ssid1) { throw new NotImplementedException(); }
        public void RemoveUser(Guid guid) { throw new NotImplementedException(); }

        public bool TryGetUser(Guid guid, out int userId)
        {
            userId = default;

            if (_session.TryGetValue(guid, out var value))
            {
                userId = value.Id;
                return true;
            }
            else return false;
        }
    }
}
