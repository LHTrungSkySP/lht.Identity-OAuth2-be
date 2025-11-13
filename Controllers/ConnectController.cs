using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
namespace IdentityOAuth2.Controllers
{
    public class ConnectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConnectController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("~/connect/authorize")]
        public IActionResult Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("OIDC request not found.");
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                var feDomain = _configuration["IdentityServer:FEDomain"];
                var returnUrl = $"{Request.Path}{Request.QueryString}";
                var loginUrl = $"{feDomain}/login?returnUrl={Uri.EscapeDataString(returnUrl)}";
                return Redirect(loginUrl);
            }
            var claims = new List<Claim> {
            new Claim(Claims.Subject, User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.Identity!.Name!),
            new Claim(Claims.Name, User.Identity!.Name ?? "user"),
            new Claim(Claims.Email, User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "") };
            var identity = new ClaimsIdentity(claims, "pwd", Claims.Name, Claims.Role);
            identity.SetScopes(request.GetScopes());
            var principal = new ClaimsPrincipal(identity);
            principal.SetResources("api");
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        //[HttpGet("~/login")]
        //public IActionResult Login([FromQuery] string returnUrl)
        //{
        //    if (string.IsNullOrEmpty(returnUrl))
        //    {
        //        returnUrl = "/";
        //    }


        //    // Redirect về trang login của FE
        //    var redirectUrl = $"{feDomain}/login?returnUrl={Uri.EscapeDataString(returnUrl)}";

        //    return Redirect(redirectUrl);
        //}

    }
}
