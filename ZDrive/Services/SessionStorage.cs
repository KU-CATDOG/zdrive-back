using System.Collections.ObjectModel;

namespace ZDrive.Services;

public interface ISessionStorage
{
    ReadOnlyDictionary<Guid, Session> Session { get; }

    bool AddSession(int userId, out Guid ssid);
    bool AddSession(int userId, int authority, out Guid ssid);
    bool AddSession(int userId, DateTime dateTime, out Guid ssid1);
    bool TryGetUser(Guid guid, out int userId);
    void RemoveUser(Guid guid);
    void RemoveUser(int userId);
}

public class SessionStorage : ISessionStorage
{
    private readonly Dictionary<Guid, Session> session = new Dictionary<Guid, Session>();
    private static readonly TimeSpan expires = new TimeSpan(1, 0, 0);

    public ReadOnlyDictionary<Guid, Session> Session => session.AsReadOnly();

    public bool AddSession(int userId, out Guid ssid)
    {
        RemoveExpiredUser();

        foreach (var v in session.Values)
            if (v.Id == userId)
            {
                ssid = default;
                return false;
            }

        ssid = Guid.NewGuid();
        session[ssid] = new Session(userId, DateTime.Now.Add(expires));

        return true;
    }

    public bool AddSession(int userId, int authority, out Guid ssid)
    {
        RemoveExpiredUser();

        foreach (var v in session.Values)
            if (v.Id == userId)
            {
                ssid = default;
                return false;
            }

        ssid = Guid.NewGuid();
        session[ssid] = new Session(userId, authority, DateTime.Now.Add(expires));

        return true;
    }

    public bool AddSession(int userId, DateTime dateTime, out Guid ssid)
    {
        RemoveExpiredUser();

        foreach (var v in session.Values)
            if (v.Id == userId)
            {
                ssid = default;
                return false;
            }

        ssid = Guid.NewGuid();
        session[ssid] = new Session(userId, dateTime);

        return true;
    }

    public void RemoveUser(Guid ssid)
    {
        if (!session.ContainsKey(ssid)) throw new KeyNotFoundException();
        var ret = session.Remove(ssid);
    }

    public void RemoveUser(int userId)
    {
        var buffer = new List<Guid>();
        foreach (var kv in session)
        {
            if (kv.Value.Id == userId) buffer.Add(kv.Key);
        }
        foreach (var k in buffer)
        {
            session.Remove(k);
        }
    }

    public bool TryGetUser(Guid ssid, out int userId)
    {
        RemoveExpiredUser();
        userId = default;

        if (session.TryGetValue(ssid, out var value))
        {
            userId = value.Id;
            return true;
        }
        else return false;
    }

    private void Print()
    {
        Console.WriteLine("Session List:");
        foreach (var kv in session)
        {
            Console.WriteLine($"SSID: {kv.Key.ToString()}, UserId: {kv.Value.Id}, Expires: {kv.Value.Expires}");
        }
    }

    private void RemoveExpiredUser()
    {
        List<Guid> buffer = new List<Guid>();

        foreach (var kv in session)
        {
            if (kv.Value.IsExpired()) buffer.Add(kv.Key);
        }

        for (int i = buffer.Count - 1; i >= 0; i--)
        {
            session.Remove(buffer[i]);
        }
    }
}

public struct Session
{
    public int Id;
    public DateTime Expires;
    public int Authority = 0;

    public Session(int _Id, DateTime _Expires)
    {
        Id = _Id;
        Expires = _Expires;
    }

    public Session(int _Id, int _Authority, DateTime _Expires)
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