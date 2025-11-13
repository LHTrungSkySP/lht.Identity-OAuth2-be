using OpenIddict.Abstractions;

namespace IdentityOAuth2.Models.Request.Applications
{
    public class CreateApplicationRequest
    {
        public string ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? DisplayName { get; set; }
        public List<string> Domains { get; set; } = new List<string>();
        //public string ClientType { get; set; } = OpenIddictConstants.ClientTypes.Public;
        //public List<string> RedirectUris { get; set; } = new List<string>();
        //public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        //public List<string> Permissions { get; set; } = new List<string>();

    }
}
