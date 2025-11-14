using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendAPI.Domain.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    // ===================== LOGIN =====================
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email și parola sunt obligatorii.");

        var targetRole = request.UserType switch
        {
            "Candidat" => UserRole.Candidat,
            "Company" => UserRole.Company,
            _ => (UserRole?)null
        };

        if (targetRole == null)
            return BadRequest("Tip utilizator invalid.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == targetRole);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { Message = "Email sau parolă greșită." });

        var token = GenerateJwtToken(user);

        return Ok(new LoginResponseModel
        {
            Token = token,
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }

    // ===================== REGISTER =====================
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] Shared.DTOs.RegisterRequest registerRequest)
    {
        if (string.IsNullOrWhiteSpace(registerRequest.Email) || string.IsNullOrWhiteSpace(registerRequest.Password))
            return BadRequest(new { Message = "Email și parola sunt obligatorii." });

        var (isValid, errors) = ValidatePassword(registerRequest.Password);
        if (!isValid)
            return BadRequest(new { Message = "Parolă invalidă.", Errors = errors });

        if (await _db.Users.AnyAsync(u => u.Email == registerRequest.Email))
            return BadRequest(new { Message = "Emailul este deja înregistrat." });

        UserRole role = registerRequest.UserType;

        User newUser;

        if (role == UserRole.Candidat)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.FirstName) || string.IsNullOrWhiteSpace(registerRequest.LastName))
                return BadRequest(new { Message = "Prenumele și numele sunt obligatorii pentru candidați." });

            newUser = new Candidat
            {
                Id = Guid.NewGuid(),
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password, 12),
                Role = UserRole.Candidat,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                AboutMe = registerRequest.AboutMe ?? string.Empty,
                CvPath = string.Empty,
            };
        }
        else // Company
        {
            if (string.IsNullOrWhiteSpace(registerRequest.CompanyName))
                return BadRequest(new { Message = "Numele companiei este obligatoriu." });
            if (string.IsNullOrWhiteSpace(registerRequest.City) || string.IsNullOrWhiteSpace(registerRequest.OfficeAddress))
                return BadRequest(new { Message = "Orașul și adresa biroului sunt obligatorii." });

            newUser = new Company
            {
                Id = Guid.NewGuid(),
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password, 12),
                Role = UserRole.Company,
                CompanyName = registerRequest.CompanyName,
                City = registerRequest.City,
                OfficeAddress = registerRequest.OfficeAddress,
                OfficePhone = registerRequest.OfficePhone ?? string.Empty,
                JobIds = new List<Guid>(),
            };
        }

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        var token = GenerateJwtToken(newUser);

        return Ok(new LoginResponseModel
        {
            Token = token,
            Id = newUser.Id,
            Email = newUser.Email,
            Role = newUser.Role.ToString(),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }

    // ===================== PROFILE =====================
    [Authorize]
    [HttpGet("Profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return Unauthorized();

        var user = await _db.Users.FindAsync(userGuid);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            Role = user.Role.ToString()
        });
    }

    // ===================== REFRESH TOKEN =====================
    [Authorize]
    [HttpPost("RefreshToken")]
    public IActionResult RefreshToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            return Unauthorized();

        var user = new User
        {
            Id = Guid.Parse(userId),
            Email = email,
            Role = Enum.Parse<UserRole>(role)
        };

        var newToken = GenerateJwtToken(user);

        return Ok(new
        {
            Token = newToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }

    // ===================== CHANGE PASSWORD =====================
    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return Unauthorized();

        var user = await _db.Users.FindAsync(userGuid);
        if (user == null)
            return NotFound("Utilizator negăsit.");

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            return BadRequest("Parola veche este incorectă.");

        var (isValid, errors) = ValidatePassword(request.NewPassword);
        if (!isValid)
            return BadRequest(new { Message = "Parolă nouă invalidă.", Errors = errors });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, 12);
        await _db.SaveChangesAsync();

        return Ok(new { Message = "Parola a fost schimbată cu succes." });
    }

    // ===================== HELPER METHODS =====================
    private (bool IsValid, List<string> Errors) ValidatePassword(string password)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Parola este obligatorie.");
            return (false, errors);
        }
        if (password.Length < 6)
            errors.Add("Parola trebuie să conțină minim 6 caractere.");
        if (password.Length > 128)
            errors.Add("Parola este prea lungă (max 128 caractere).");

        return (errors.Count == 0, errors);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ===================== DTOs =====================
    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }

    public class LoginResponseModel
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
