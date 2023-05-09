using System.Collections.ObjectModel;
using ZDrive.Models;

namespace ZDrive.Services;

public interface ISessionStorage
{
    ReadOnlyDictionary<Guid, Session> Session { get; }

    bool AddSession(int userId, out Guid ssid);
    bool AddSession(int userId, Authority authority, out Guid ssid);
    bool AddSession(int userId, DateTime dateTime, out Guid ssid1);
    bool TryGetUser(Guid guid, out Session session);
    void RemoveUser(Guid guid);
    void RemoveUser(int userId);
}

public class SessionStorage : ISessionStorage
{
    private readonly Dictionary<Guid, Session> _session = new Dictionary<Guid, Session>();
    private static readonly TimeSpan expires = new TimeSpan(1, 0, 0);

    public ReadOnlyDictionary<Guid, Session> Session => _session.AsReadOnly();

    public bool AddSession(int userId, out Guid ssid)
    {
        RemoveExpiredUser();
        
        if (FindUserById(userId))
        {
            ssid = default;
            return false;
        }

        ssid = Guid.NewGuid();
        _session[ssid] = new Session(userId, DateTime.Now.Add(expires));

        return true;
    }

    public bool AddSession(int userId, Authority authority, out Guid ssid)
    {
        RemoveExpiredUser();

        if (FindUserById(userId))
        {
            ssid = default;
            return false;
        }

        ssid = Guid.NewGuid();
        _session[ssid] = new Session(userId, authority, DateTime.Now.Add(expires));

        return true;
    }

    public bool AddSession(int userId, DateTime dateTime, out Guid ssid)
    {
        RemoveExpiredUser();

        if (FindUserById(userId))
        {
            ssid = default;
            return false;
        }

        ssid = Guid.NewGuid();
        _session[ssid] = new Session(userId, dateTime);

        return true;
    }

    private bool FindUserById(int userId)
    {
        foreach (var v in _session.Values)
            if (v.Id == userId)
                return true;

        return false;
    }

    public void RemoveUser(Guid ssid)
    {
        if (!_session.ContainsKey(ssid)) throw new KeyNotFoundException();
        var ret = _session.Remove(ssid);
    }

    public void RemoveUser(int userId)
    {
        var buffer = new List<Guid>();
        foreach (var kv in _session)
        {
            if (kv.Value.Id == userId) buffer.Add(kv.Key);
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

    private void Print()
    {
        Console.WriteLine("Session List:");
        foreach (var kv in _session)
        {
            Console.WriteLine($"SSID: {kv.Key.ToString()}, UserId: {kv.Value.Id}, Expires: {kv.Value.Expires}");
        }
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
    public int Id;
    public DateTime Expires;
    public Authority Authority = 0;

    public Session(int _Id, DateTime _Expires)
    {
        Id = _Id;
        Expires = _Expires;
    }

    public Session(int _Id, Authority _Authority, DateTime _Expires)
    {
        Id = _Id;
        Expires = _Expires;
        Authority = _Authority;
    }

    public bool IsExpired()
    {
        return DateTime.Compare(DateTime.Now, Expires) > 0;
    }
}