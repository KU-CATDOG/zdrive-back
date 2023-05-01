using ZDrive.Services;

namespace ZDrive_Test;

public class SessionStorageTest
{
    private ISessionStorage CreateSessionStorage()
        => new SessionStorage();

    [Test]
    public void AddSession_NewUser_ShouldBeSavedInDictionary()
    {
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        var ret = sessionStorage.AddSession(userId, out var ssid);

        Assert.IsTrue(ret);
        Assert.AreNotEqual(default(Guid), ssid);
    }

    [Test]
    public void AddSession_ExistUser_ReturnsFalse()
    {
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, out var ssid1);
        var ret = sessionStorage.AddSession(userId, out var ssid2);

        Assert.IsFalse(ret);
        Assert.AreEqual(default(Guid), ssid2);
    }

    [Test]
    public void AddSession_ExpiredUser_ShouldBeRemoved()
    {
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, DateTime.Now.AddHours(-1), out var ssid1);
        sessionStorage.AddSession(userId + 1, out var ssid2);

        Assert.False(sessionStorage.Session.ContainsKey(ssid1));
    }

    [Test]
    public void TryGetUser_ExistUser_ReturnsId()
    {
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, out var ssid1);

        Assert.True(sessionStorage.TryGetUser(ssid1, out var ret));
        Assert.AreEqual(userId, ret);
    }

    [Test]
    public void TryGetUser_EmptyUser_ReturnsNull()
    {
        var sessionStorage = CreateSessionStorage();

        Assert.False(sessionStorage.TryGetUser(Guid.NewGuid(), out var ret));
        Assert.AreEqual(default(int), ret);
    }

    [Test]
    public void TryGetUser_ExpiredUser_ReturnsNull()
    {
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, DateTime.Now.AddHours(-1), out var ssid1);

        Assert.False(sessionStorage.TryGetUser(ssid1, out var ret));
        Assert.AreEqual(default(int), ret);
    }

    [Test]
    public void RemoveUser_ExistSSID_RemovesSSID()
    {
        var sessionStorage = CreateSessionStorage();
        var userId = 1;
        sessionStorage.AddSession(userId, out var ssid);
        sessionStorage.RemoveUser(ssid);

        Assert.False(sessionStorage.Session.ContainsKey(ssid));
    }

    [Test]
    public void RemoveUser_EmptySSID_ThrowsException()
    {
        var sessionStorage = CreateSessionStorage();
        Assert.Catch<KeyNotFoundException>(() => sessionStorage.RemoveUser(Guid.NewGuid()));
    }
}