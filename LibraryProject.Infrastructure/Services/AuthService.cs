using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly Dictionary<string, (string Password, string Role)> _users = new()
    {
        { "admin", ("adminpass", "Admin") },
        { "librarian", ("libpass", "Librarian") }
    };

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public Task<string?> Authenticate(string username, string password)
    {
        if (_users.TryGetValue(username, out var user) && user.Password == password)
        {
            var token = GenerateJwtToken(username, user.Role);
            return Task.FromResult<string?>(token);
        }
        return Task.FromResult<string?>(null);
    }

    private string GenerateJwtToken(string username, string role)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
