using BackendAPI.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

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
        Console.WriteLine("=== LOGIN REQUEST ===");
        Console.WriteLine("Email: " + request.Email);
        Console.WriteLine("UserType: " + request.UserType);
    
        User user = null;
    
        if (request.UserType == "Candidat")
        { 
            user = await _db.Users.FirstOrDefaultAsync(u => 
                u.Email == request.Email && 
                u.Role == UserRole.Candidat);
        }
        else if (request.UserType == "Company")
        { 
            user = await _db.Users.FirstOrDefaultAsync(u => 
                u.Email == request.Email && 
                u.Role == UserRole.Company);
        }
        else
        {
            return BadRequest("Tip utilizator invalid.");
        }
    
        if (user == null)
        {
            Console.WriteLine("User not found in database!");
            return Unauthorized("Email sau parolă greșită.");
        }

        Console.WriteLine("User found: " + user.Email);
        
        return Ok(new 
        { 
            Message = "Login successful",
            Id = user.Id,
            Role = user.Role.ToString()
        });
    }
    
    // --- DTO classes ---
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }
    
}
