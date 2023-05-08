using ZDrive.Models;

namespace ZDrive.Services;

public interface IAuthorizationManager
{
    public IResult CheckSession(HttpRequest request, out int userId);
    public IResult CheckSession(HttpRequest request, Authority authority, out int userId);
}

public class AuthorizationManager : IAuthorizationManager
{
    private readonly ISessionStorage _sessionStorage;

    public AuthorizationManager(ISessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public IResult CheckSession(HttpRequest request, out int userId)
    {
        userId = default;
        var ssid = request.Cookies["sessionId"];
        if (ssid == null) return Results.Unauthorized();
        if (!Guid.TryParse(ssid, out var guid)) return Results.BadRequest();
        if (!_sessionStorage.TryGetUser(guid, out var session)) return Results.Unauthorized();
        userId = session.Id;

        return Results.Ok();
    }

    public IResult CheckSession(HttpRequest request, Authority authority, out int userId)
    {
        userId = default;
        var ssid = request.Cookies["sessionId"];
        if (ssid == null) return Results.Unauthorized();
        if (!Guid.TryParse(ssid, out var guid)) return Results.BadRequest();
        if (!_sessionStorage.TryGetUser(guid, out var session)) return Results.Unauthorized();
        if ((int)session.Authority < (int)authority) return Results.Forbid();

        return Results.Ok();
    }
}