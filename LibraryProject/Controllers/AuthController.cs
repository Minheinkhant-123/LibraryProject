using LibraryProject.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel model)
    {
        var token = await _authService.Authenticate(model.Username, model.Password);
        if (token == null)
            return Unauthorized(new { message = "Invalid username or password" });

        return Ok(new { token });
    }
}
