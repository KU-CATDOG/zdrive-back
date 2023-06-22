using System.Collections.ObjectModel;
using ZDrive.Models;

namespace ZDrive.Services;

public interface ISessionStorage
{
    ReadOnlyDictionary<Guid, Session> Session { get; }

    bool AddSession(UserData userData, out Guid ssid, DateTime dateTime = default);
    bool TryGetUser(Guid guid, out Session session);
    void RemoveUser(Guid guid);
    void RemoveUser(UserData userData);
}

public class SessionStorage : ISessionStorage
{
    private readonly Dictionary<Guid, Session> _session = new Dictionary<Guid, Session>();
    private static readonly TimeSpan expires = new TimeSpan(1, 0, 0);

    public ReadOnlyDictionary<Guid, Session> Session => _session.AsReadOnly();

    public bool AddSession(UserData userData, out Guid ssid, DateTime dateTime = default)
    {
        RemoveExpiredUser();

        ssid = Guid.NewGuid();
        _session[ssid] =
            new Session(userData, dateTime == default ? DateTime.Now.Add(expires) : dateTime);

        return true;
    }

    public void RemoveUser(Guid ssid)
    {
        if (!_session.ContainsKey(ssid)) throw new KeyNotFoundException();
        var ret = _session.Remove(ssid);
    }

    public void RemoveUser(UserData userData)
    {
        var buffer = new List<Guid>();
        foreach (var kv in _session)
        {
            if (kv.Value.Data == userData) buffer.Add(kv.Key);
        }
        foreach (var k in buffer)
        {
            _session.Remove(k);
        }
    }

    public bool TryGetUser(Guid ssid, out Session session)
    {
        RemoveExpiredUser();
        session = default;

        if (_session.TryGetValue(ssid, out var value))
        {
            session = value;
            return true;
        }
        else return false;
    }

    private void RemoveExpiredUser()
    {
        List<Guid> buffer = new List<Guid>();

        foreach (var kv in _session)
        {
            if (kv.Value.IsExpired()) buffer.Add(kv.Key);
        }

        for (int i = buffer.Count - 1; i >= 0; i--)
        {
            _session.Remove(buffer[i]);
        }
    }
}

public struct Session
{
    public UserData Data;
    public DateTime Expires;

    public Session(UserData data, DateTime _Expires)
    {
        Data = data;
        Expires = _Expires;
    }

    public bool IsExpired()
    {
        return DateTime.Compare(DateTime.Now, Expires) > 0;
    }
}
