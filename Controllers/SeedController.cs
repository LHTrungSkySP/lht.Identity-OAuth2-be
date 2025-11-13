using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSC.Identity.Models;
using MSC.Identity.Models.Entities;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MSC.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        readonly ApplicationDbContext _context;
        readonly UserManager<IdentityUser> _userManager;
        readonly IOpenIddictApplicationManager _openIddictApplicationManager;
        readonly IConfiguration _configuration;

        public SeedController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            IOpenIddictApplicationManager openIddictApplicationManager,
            IConfiguration configuration
            )
        {
            _context = context;
            _userManager = userManager;
            _openIddictApplicationManager = openIddictApplicationManager;
            _configuration = configuration;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            var alice = await _userManager.FindByEmailAsync("lehoangtrung038582@gmail.vn");
            if (alice == null)
            {
                alice = new IdentityUser { UserName = "admin", Email = "lehoangtrung038582@gmail.vn", EmailConfirmed = true };
                var ret = await _userManager.CreateAsync(alice, "Admin1!");
            }

            var adminClient = await _openIddictApplicationManager.FindByClientIdAsync(_configuration["ApplicationClients:AdminClientId"]);
            if (await _openIddictApplicationManager.FindByClientIdAsync("AERP") is null)
            {
                await _openIddictApplicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = _configuration["ApplicationClients:AdminClientId"],
                    ClientSecret = _configuration["ApplicationClients:AdminClientSecret"],
                    DisplayName = "AERP Client",
                    ClientType = ClientTypes.Confidential,
                    RedirectUris = { new Uri($"{_configuration["ApplicationClients:AdminBEUrl"]}/swagger/oauth2-redirect.html"), new Uri($"{_configuration["ApplicationClients:AdminFEUrl"]}/oauth2-callback") },
                    PostLogoutRedirectUris = { new Uri($"{_configuration["ApplicationClients:AdminFEUrl"]}/signout-callback-oidc") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Introspection,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.ResponseTypes.Code,
                        Permissions.Prefixes.Scope + "openid",
                        Permissions.Prefixes.Scope + "profile",
                        Permissions.Prefixes.Scope + "api",
                        Permissions.Prefixes.Scope + "offline_access"
                    }
                });
            }

            return Ok(new { ok = true });
        }
    }
}
