using BackendAPI.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class RequestController : ControllerBase
{
    private readonly ApplicationDbContext dbcontext;

    public RequestController(ApplicationDbContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    [HttpGet]
    public IActionResult GetAllRequests()
    {
        return Ok(dbcontext.Requests.ToList());
    }
    
    [HttpGet("{id}")]
    public IActionResult GetAllRequestForCandidat(Guid id)
    {
        var result = dbcontext.Requests.Where(r => r.UserId == id).ToList();
        return Ok(result);
    }
    [HttpPost]
    public IActionResult CreateRequest([FromBody] RequestDto requestDto)
    {
        if (requestDto == null)
            return BadRequest("Invalid request data.");

        var request = new Request()
        {
            Id = Guid.NewGuid(),
            JobId = requestDto.JobId,
            UserId = requestDto.UserId,
            Status = requestDto.Status
            
        };

        dbcontext.Requests.Add(request);
        dbcontext.SaveChanges();

        return Ok(request);
    }
    
    [HttpPost("UploadCv")]
    public async Task<IActionResult> UploadCv([FromForm] IFormFile cvFile, [FromForm] string FullName, [FromForm] string Email, [FromForm] Guid JobId, [FromForm] Guid UserId)
    {
        if (cvFile == null || cvFile.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, cvFile.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await cvFile.CopyToAsync(stream);
        }

        var candidat = new Candidat
        {
            FirstName = FullName.Split(' ')[0],
            LastName = FullName.Split(' ').Length > 1 ? FullName.Split(' ')[1] : "",
            Email = Email,
            CvPath = filePath,
            Role = UserRole.Candidat
        };

        dbcontext.Users.Add(candidat);
        await dbcontext.SaveChangesAsync();

        var request = new Request
        {
            JobId = JobId,
            UserId = UserId,
            Status = Status.OnStayding
        };

        dbcontext.Requests.Add(request);
        await dbcontext.SaveChangesAsync();

        return Ok(new { message = "CV uploaded and request created successfully!" });
    }


   
}