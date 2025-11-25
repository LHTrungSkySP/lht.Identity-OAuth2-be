using OpenIddict.Abstractions;

namespace IdentityOAuth2.Models.Request.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
