using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ZDrive.Services;
public class SessionTokenAuthenticationSchemeHandler : AuthenticationHandler<SessionTokenAuthenticationSchemeOptions>
{
    private readonly ISessionStorage _session;

    public SessionTokenAuthenticationSchemeHandler(
        IOptionsMonitor<SessionTokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ISessionStorage session) : base(options, logger, encoder, clock)
    {
        _session = session;
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {

        if (Request.Cookies["sessionId"] == null)
            return AuthenticateResult.Fail("Session authentication failed.  Authorization header is missing.");
        if (!Guid.TryParse(Request.Cookies["sessionId"], out var guid))
            return AuthenticateResult.Fail("Session authentication failed.  Token is invalid.");
        if (!_session.TryGetUser(guid, out var session))
            return AuthenticateResult.Fail("Session authentication failed.  Token does not exist in storage.");

        var claims = new[] { new Claim("Id", session.Id.ToString()) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}