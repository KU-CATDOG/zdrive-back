using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive_Test;

public class SessionStorageTest
{
    private ISessionStorage CreateSessionStorage()
        => new SessionStorage();

    private User CreateRandomUser()
        => new TestDataBuilder<User>().Randomize().Build();

    [Test]
    public void AddSession_NewUser_ShouldBeSavedInDictionary()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userData = new UserData(CreateRandomUser());

        // Act
        var ret = sessionStorage.AddSession(userData, out var ssid);

        // Assert
        Assert.IsTrue(ret);
        Assert.AreNotEqual(default(Guid), ssid);
    }

    [Test]
    public void AddSession_ExpiredUser_ShouldBeRemoved()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userData = new UserData(CreateRandomUser());

        // Act
        sessionStorage.AddSession(userData, out var ssid, DateTime.Now.AddHours(-1));
        sessionStorage.AddSession(userData, out var ssid2);

        // Assert
        Assert.False(sessionStorage.Session.ContainsKey(ssid));
    }

    [Test]
    public void TryGetUser_ExistUser_ReturnsId()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userData = new UserData(CreateRandomUser());

        // Act
        sessionStorage.AddSession(userData, out var ssid1);

        // Assert
        Assert.True(sessionStorage.TryGetUser(ssid1, out var ret));
        Assert.AreEqual(userData, ret.Data);
    }

    [Test]
    public void TryGetUser_EmptyUser_ReturnsNull()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();

        // Assert
        Assert.False(sessionStorage.TryGetUser(Guid.NewGuid(), out var ret));
        Assert.AreEqual(default(UserData), ret.Data);
    }

    [Test]
    public void TryGetUser_ExpiredUser_ReturnsNull()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userData = new UserData(CreateRandomUser());

        // Act
        sessionStorage.AddSession(userData, out var ssid1, DateTime.Now.AddHours(-1));

        // Assert
        Assert.False(sessionStorage.TryGetUser(ssid1, out var ret));
        Assert.AreEqual(default(UserData), ret.Data);
    }

    [Test]
    public void RemoveUser_ExistSSID_RemovesSSID()
    {
        // Arrange
        var sessionStorage = CreateSessionStorage();
        var userData = new UserData(CreateRandomUser());
        sessionStorage.AddSession(userData, out var ssid);

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
        var userData = new UserData(CreateRandomUser());
        sessionStorage.AddSession(userData, out var ssid);

        // Act
        sessionStorage.RemoveUser(userData);

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