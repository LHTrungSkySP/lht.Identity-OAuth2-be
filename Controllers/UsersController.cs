using IdentityOAuth2.Models.Request.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/applications
    [HttpGet("search")]
    public async Task<IActionResult> GetAll()
    {
        var list = new List<object>();
        return Ok(_userManager.Users.ToList());
    }

    // GET: api/applications/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var app = await _userManager.FindByIdAsync(id);
        if (app == null) return NotFound();

        return Ok(app);
    }

    // POST: api/applications
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
        };
        user.EmailConfirmed = true;
        var ret = await _userManager.CreateAsync(user, dto.Password);
        return Ok(ret);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateUserRequest dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        await _userManager.UpdateAsync(user);

        return Ok(new { message = "Application updated successfully." });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var app = await _userManager.FindByIdAsync(id);
        if (app == null) return NotFound();

        await _userManager.DeleteAsync(app);

        return Ok(new { message = "Application deleted successfully." });
    }
}
