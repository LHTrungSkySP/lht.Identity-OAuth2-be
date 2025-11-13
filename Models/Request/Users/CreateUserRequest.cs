using OpenIddict.Abstractions;

namespace IdentityOAuth2.Models.Request.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; } = true;
        public string Password { get; set; }
    }
}
