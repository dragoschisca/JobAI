using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendAPI.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BackendAPI.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Afișează ce vine în request
        Console.WriteLine("=== LOGIN REQUEST ===");
        Console.WriteLine("Email: " + request.Email);
        Console.WriteLine("Password: " + request.Password);

        // Verifică dacă user există în baza de date
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            Console.WriteLine("User not found in database!");
            return Unauthorized("Email sau parolă greșită.");
        }
    
        Console.WriteLine("User found: " + user.Email);

        // Verifică parola
        if (user.Password != request.Password)
        {
            Console.WriteLine("Password mismatch!");
            return Unauthorized("Email sau parolă greșită.");
        }

        Console.WriteLine("Login successful!");

        // Returnează date pentru frontend
        return Ok(new
        {
            Message = "Login reușit!",
            Name = user.Email
        });
    }

    // --- DTO classes ---
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
}
