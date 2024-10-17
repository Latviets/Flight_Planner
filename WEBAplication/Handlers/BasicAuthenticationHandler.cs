using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace WebApplication1.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(Authorize());
        }

        private AuthenticateResult Authorize()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization header");

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];

                if (username != "codelex-admin" && password != "Password123")
                {
                    return AuthenticateResult.Fail("Invalid username or password.");
                }

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username),
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
