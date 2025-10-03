using BackendAPI.Domain.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RequestController : ControllerBase
{
    private readonly ApplicationDbContext _dbcontext;

    public RequestController(ApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    [HttpGet]
    public IActionResult GetAllRequests()
    {
        return Ok(_dbcontext.Requests.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetAllRequestForCandidat(Guid id)
    {
        var result = _dbcontext.Requests.Where(r => r.UserId == id).ToList();
        return Ok(result);
    }

    [HttpPost("UploadCv")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> UploadCv(
        [FromForm] IFormFile cvFile, 
        [FromForm] string fullName, 
        [FromForm] string email, 
        [FromForm] Guid jobId, 
        [FromForm] Guid userId)
    {
        Console.WriteLine("🚀 Entered UploadCv");

        if (cvFile == null || cvFile.Length == 0)
        {
            Console.WriteLine("⚠ cvFile is null or empty");
            return BadRequest("No file uploaded.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Console.WriteLine("📂 Creating uploads folder");
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{cvFile.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await cvFile.CopyToAsync(stream);
        }
        Console.WriteLine($"📄 File saved: {filePath}");

        var names = fullName.Split(' ', 2, StringSplitOptions.TrimEntries);
        var candidat = new Candidat
        {
            FirstName = names[0],
            LastName = names.Length > 1 ? names[1] : "",
            Email = email,
            CvPath = filePath,
            Role = UserRole.Candidat
        };

        _dbcontext.Users.Add(candidat);
        await _dbcontext.SaveChangesAsync();
        Console.WriteLine($"👤 Candidate created: {candidat.FirstName} {candidat.LastName}, Id={candidat.Id}");

        var request = new Request
        {
            JobId = jobId,
            UserId = candidat.Id,
            Status = Status.OnStayding
        };

        _dbcontext.Requests.Add(request);
        await _dbcontext.SaveChangesAsync();
        Console.WriteLine($"📑 Request created for JobId={jobId} UserId={candidat.Id}");

        Console.WriteLine("🏁 UploadCv completed");
        return Ok(new { message = "CV uploaded and request created successfully!" });
    }

}
