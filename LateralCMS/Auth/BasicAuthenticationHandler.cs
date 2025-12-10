using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace LateralCMS.Auth;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IConfiguration _configuration = configuration;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out StringValues value) ||
            !value.ToString().StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing or Invalid Authorization Header"));
        }

        var authHeader = value.ToString();

        var token = authHeader.Substring("Basic ".Length).Trim();
        var credentialBytes = Convert.FromBase64String(token);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
        var username = credentials[0];
        var password = credentials[1];

        var users = _configuration.GetSection("Authentication:Users").Get<List<AuthUser>>();

        if (users == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("No users configured"));
        }

        var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
        if (user.Roles != null)
        {
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}