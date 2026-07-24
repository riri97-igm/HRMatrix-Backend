using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;

namespace IdentityService.Controllers


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthController(AppDbContext db, IJwtService _jwtService)
    {
        _db = db;
        _jwtService = JwtService;
    }
   [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        //Find activer user by email
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        //Verify password hash
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))

            return Unauthorized(new { message = "Invalid email or password" });

        // Generate JWT token 
        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id
        });
    }

    [HttpPost("register")]
    [Authorize (Roles = "Admin")]
    public async Task<IActionResult>
}
