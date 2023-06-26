using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive_Test;

public class SessionTokenAuthenticationSchemeHandlerTest
{
    private Mock<IOptionsMonitor<SessionTokenAuthenticationSchemeOptions>> _options = null!;
    private Mock<ILoggerFactory> _loggerFactory = null!;
    private Mock<UrlEncoder> _encoder = null!;
    private Mock<ISystemClock> _clock = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private SessionTokenAuthenticationSchemeHandler _handler = null!;

    private SessionTokenAuthenticationSchemeHandler GetHandler()
        => new SessionTokenAuthenticationSchemeHandler(_options.Object, _loggerFactory.Object, _encoder.Object, _clock.Object, _sessionStorage.Object);

    private Session CreateRandomSession()
        => new Session(UserData.User(new TestDataBuilder<User>().Randomize().Build()), DateTime.Now);

    [SetUp]
    public void SetUp()
    {
        _options = new Mock<IOptionsMonitor<SessionTokenAuthenticationSchemeOptions>>();

        // This Setup is required for .NET Core 3.1 onwards.
        _options
            .Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new SessionTokenAuthenticationSchemeOptions());

        var logger = new Mock<ILogger<SessionTokenAuthenticationSchemeHandler>>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

        _encoder = new Mock<UrlEncoder>();
        _clock = new Mock<ISystemClock>();
        _sessionStorage = new Mock<ISessionStorage>();
    }


    [Test]
    public async Task HandleAuthenticateAsync_NoSSIDCookie_ReturnsAuthenticateResultFail()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _handler = GetHandler();

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme("SessionTokens", null, typeof(SessionTokenAuthenticationSchemeHandler)), context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Session authentication failed.  Authorization header is missing."));
    }

    [Test]
    public async Task HandleAuthenticateAsync_InvalidSSID_ReturnsAuthenticateResultFail()
    {
        // Arrange
        var context = new Mock<HttpContext>();
        context.Setup(e => e.Request.Cookies["sessionId"])
            .Returns("invalidSSID");
        _handler = GetHandler();

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme("SessionTokens", null, typeof(SessionTokenAuthenticationSchemeHandler)), context.Object);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Session authentication failed.  Token is invalid."));
    }

    [Test]
    public async Task HandleAuthenticateAsync_NonSSID_ReturnsAuthenticateResultFail()
    {
        // Arrange
        var ssid = Guid.NewGuid();
        var session = CreateRandomSession();
        var context = new Mock<HttpContext>();
        context.Setup(e => e.Request.Cookies["sessionId"])
            .Returns(ssid.ToString());

        _sessionStorage.Setup(e => e.TryGetUser(It.IsAny<Guid>(), out session))
            .Returns(false);

        _handler = GetHandler();

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme("SessionTokens", null, typeof(SessionTokenAuthenticationSchemeHandler)), context.Object);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Session authentication failed.  Token does not exist in storage."));
    }

    [Test]
    public async Task HandleAuthenticateAsync_ValidToken_ReturnsAuthenticateResultSuccessAsync()
    {
        // Arrange
        var ssid = Guid.NewGuid();
        var session = CreateRandomSession();
        var context = new Mock<HttpContext>();
        context.Setup(e => e.Request.Cookies["sessionId"])
            .Returns(ssid.ToString());

        _sessionStorage.Setup(e => e.TryGetUser(It.Is<Guid>(e => e != ssid), out session))
            .Returns(false);
        _sessionStorage.Setup(e => e.TryGetUser(ssid, out session))
            .Returns(true);

        _handler = GetHandler();

        // Act
        await _handler.InitializeAsync(new AuthenticationScheme("SessionTokens", null, typeof(SessionTokenAuthenticationSchemeHandler)), context.Object);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Ticket?.Principal.HasClaim(c => c.Type == ClaimTypes.Sid && c.Value == session.Data.Id.ToString()), Is.True);
    }
}
