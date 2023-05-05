namespace ZDrive.Services;

public interface IAuthorizationManager
{
    public IResult CheckSession(HttpRequest request, out int userId);
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
        if (!_sessionStorage.TryGetUser(guid, out userId)) return Results.Unauthorized();

        return Results.Ok();
    }
}