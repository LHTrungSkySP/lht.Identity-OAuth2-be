using IdentityOAuth2.Models.Request.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityOAuth2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        // SignInManager<T>: thực hiện logic đăng nhập/đăng xuất, quản lý cookie xác thực.
        private readonly SignInManager<IdentityUser> _signInManager;
        // UserManager<T>: quản lý thông tin người dùng (CRUD, check password, lockout, role, claim…).
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> sm, UserManager<IdentityUser> um)
        {
            _signInManager = sm;
            _userManager = um;
        }
        public record LoginDto(string Username, string Password);
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username) ?? await _userManager.FindByEmailAsync(dto.Username);
            if (user == null) return Unauthorized();
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, true, false); // isPersistent: lưu cookie lâu dài trong set cookie
            if (!result.Succeeded) return Unauthorized();
            return Ok(new { ok = true });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { ok = true });
        }
    }
}
