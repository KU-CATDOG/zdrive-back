using ZDrive.Services;

namespace ZDrive_Test;

public class SessionStorageTest
{
    private ISessionStorage CreateSessionStorage()
        => new SessionStorage();

    [Test]
    public void AddSession_NewUser_ShouldBeSavedInDictionary()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userId = 1;

        // Act
        var ret = sessionStorage.AddSession(userId, out var ssid);

        // Assert
        Assert.IsTrue(ret);
        Assert.AreNotEqual(default(Guid), ssid);
    }

    [Test]
    public void AddSession_ExpiredUser_ShouldBeRemoved()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, DateTime.Now.AddHours(-1), out var ssid1);

        // Act
        sessionStorage.AddSession(userId + 1, out var ssid2);

        // Assert
        Assert.False(sessionStorage.Session.ContainsKey(ssid1));
    }

    [Test]
    public void TryGetUser_ExistUser_ReturnsId()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userId = 1;

        // Act
        sessionStorage.AddSession(userId, out var ssid1);

        // Assert
        Assert.True(sessionStorage.TryGetUser(ssid1, out var ret));
        Assert.AreEqual(userId, ret.Id);
    }

    [Test]
    public void TryGetUser_EmptyUser_ReturnsNull()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();

        // Assert
        Assert.False(sessionStorage.TryGetUser(Guid.NewGuid(), out var ret));
        Assert.AreEqual(default(int), ret.Id);
    }

    [Test]
    public void TryGetUser_ExpiredUser_ReturnsNull()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userId = 1;

        // Act
        sessionStorage.AddSession(userId, DateTime.Now.AddHours(-1), out var ssid1);

        // Assert
        Assert.False(sessionStorage.TryGetUser(ssid1, out var ret));
        Assert.AreEqual(default(int), ret.Id);
    }

    [Test]
    public void RemoveUser_ExistSSID_RemovesSSID()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, out var ssid);

        // Act
        sessionStorage.RemoveUser(ssid);

        // Assert
        Assert.False(sessionStorage.Session.ContainsKey(ssid));
    }

    [Test]
    public void RemoveUser_ExistUserId_RemovesSession()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, out var ssid);

        // Act
        sessionStorage.RemoveUser(userId);

        // Assert
        Assert.False(sessionStorage.Session.ContainsKey(ssid));
    }

    [Test]
    public void RemoveUser_EmptySSID_ThrowsException()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();

        // Assert
        Assert.Catch<KeyNotFoundException>(() => sessionStorage.RemoveUser(Guid.NewGuid()));
    }
}