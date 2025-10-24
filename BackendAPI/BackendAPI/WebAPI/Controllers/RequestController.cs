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
    public IActionResult GetAllJobRequestForCandidat(Guid id)
    {
        //in requests sa gasesc toate aplicarile candidatului la joburi
        var userApplies = _dbcontext.Requests.Where(apply => apply.UserId == id).ToList();
        
        //stochez intr-o lista id-urile
        var userAppliesId = userApplies.Select(app => app.JobId);

        //extrag din jobs fiecare aplicare dupa id
        var userJobs = _dbcontext.Jobs.Where(job => userAppliesId.Contains(job.Id)).ToList<Job>();
        //sa returnez lista de joburi la care a aplicat
        return Ok(userJobs);
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

        var request = new Request
        {
            JobId = jobId,
            UserId = userId,
            Status = Status.OnStayding
        };

        _dbcontext.Requests.Add(request);
        await _dbcontext.SaveChangesAsync();
        Console.WriteLine($"📑 Request created for JobId={jobId} UserId={userId}");

        Console.WriteLine("🏁 UploadCv completed");
        return Ok(new { message = "CV uploaded and request created successfully!" });
    }

}
