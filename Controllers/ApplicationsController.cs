using IdentityOAuth2.Models.Request.Applications;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public ApplicationsController(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    // GET: api/applications
    [HttpGet("search")]
    public async Task<IActionResult> GetAll()
    {
        var list = new List<object>();
        await foreach (var app in _applicationManager.ListAsync())
        {
            list.Add(new
            {
                ClientId = await _applicationManager.GetClientIdAsync(app),
                DisplayName = await _applicationManager.GetDisplayNameAsync(app),
                RedirectUris = await _applicationManager.GetRedirectUrisAsync(app),
                PostLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(app),
                Permissions = await _applicationManager.GetPermissionsAsync(app),
            });
        }

        return Ok(list);
    }

    // GET: api/applications/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var app = await _applicationManager.FindByIdAsync(id);
        if (app == null) return NotFound();

        return Ok(new
        {
            Id = await _applicationManager.GetIdAsync(app),
            ClientId = await _applicationManager.GetClientIdAsync(app),
            DisplayName = await _applicationManager.GetDisplayNameAsync(app),
            Type = await _applicationManager.GetClientTypeAsync(app),
            Permissions = await _applicationManager.GetPermissionsAsync(app),
            RedirectUris = await _applicationManager.GetRedirectUrisAsync(app),
            PostLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(app),
            ConsentType = await _applicationManager.GetConsentTypeAsync(app)
        });
    }

    // POST: api/applications
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest dto)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = dto.ClientId,
            ClientSecret = dto.ClientSecret,
            DisplayName = dto.DisplayName,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit
        };
        foreach (var uri in dto.Domains)
        {
            descriptor.RedirectUris.Add(new Uri("https://" + uri + "/oauth2-callback"));
            descriptor.PostLogoutRedirectUris.Add(new Uri("https://" + uri + "/signout-callback-oidc"));
        }

        List<string> defaultPermissions = new List<string>
        {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Token,
            Permissions.Endpoints.Introspection, // cho phép token được kiểm tra tính hợp lệ
            Permissions.GrantTypes.AuthorizationCode, // cho phép sử dụng mã ủy quyền để lấy token
            Permissions.GrantTypes.RefreshToken, // cho phép sử dụng refresh token để lấy token mới
            Permissions.GrantTypes.ClientCredentials, // cho phép sử dụng client credentials để lấy token
            Permissions.ResponseTypes.Code,
            Permissions.Prefixes.Scope + "openid",
            Permissions.Prefixes.Scope + "profile",
            Permissions.Prefixes.Scope + "api",
            Permissions.Prefixes.Scope + "offline_access"
        };

        foreach (var permission in defaultPermissions)
            descriptor.Permissions.Add(permission);

        await _applicationManager.CreateAsync(descriptor);

        return Ok(new { message = "Application created successfully." });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateApplicationRequest dto)
    {
        var application = await _applicationManager.FindByIdAsync(id);
        if (application == null)
        {
            return NotFound(new { message = "Application not found." });
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = dto.ClientId,
            DisplayName = dto.DisplayName,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit
        };

        if (!string.IsNullOrEmpty(dto.ClientSecret))
        {
            descriptor.ClientSecret = dto.ClientSecret;
        }

        // RedirectUris
        descriptor.RedirectUris.Clear();
        descriptor.PostLogoutRedirectUris.Clear();
        foreach (var uri in dto.Domains)
        {
            descriptor.RedirectUris.Add(new Uri("https://" + uri + "/oauth2-callback"));
            descriptor.PostLogoutRedirectUris.Add(new Uri("https://" + uri + "/signout-callback-oidc"));
        }
        // Permissions
        descriptor.Permissions.Clear();
        List<string> defaultPermissions = new()
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
        };
        foreach (var permission in defaultPermissions)
            descriptor.Permissions.Add(permission);

        await _applicationManager.UpdateAsync(application, descriptor);

        return Ok(new { message = "Application updated successfully." });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var app = await _applicationManager.FindByIdAsync(id);
        if (app == null) return NotFound();

        await _applicationManager.DeleteAsync(app);

        return Ok(new { message = "Application deleted successfully." });
    }
}
