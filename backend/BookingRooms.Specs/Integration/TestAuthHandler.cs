using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingRooms.Specs.Integration;

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    public const string TestAuthenticationScheme = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var role    = Request.Headers["X-Test-Role"].FirstOrDefault()    ?? "Admin";
        var userId  = Request.Headers["X-Test-UserId"].FirstOrDefault()  ?? "test-user-id";
        var userName= Request.Headers["X-Test-UserName"].FirstOrDefault()?? "test-user";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, role)
        };

        var identity  = new ClaimsIdentity(claims, TestAuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket    = new AuthenticationTicket(principal, TestAuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}