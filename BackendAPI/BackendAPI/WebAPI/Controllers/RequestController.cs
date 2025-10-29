using System.Diagnostics;
using System.Text.Json;
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
    public async Task<IActionResult> UploadCv([FromForm] IFormFile cvFile, [FromForm] Guid jobId, [FromForm] Guid userId)
    {
        if (cvFile == null || cvFile.Length == 0)
        {
            Console.WriteLine("⚠ cvFile is null or empty");
            return BadRequest("No file uploaded.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Console.WriteLine("Creating uploads folder");
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{cvFile.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await cvFile.CopyToAsync(stream);
        }

        Console.WriteLine($"File saved: {filePath}");

        var request = new RequestDto
        {
            JobId = jobId,
            UserId = userId,
            Status = Status.OnStayding
        };
        
        var entity = new Request
        {
            JobId = request.JobId,
            UserId = request.UserId,
            Status = request.Status,
        };
        
        entity.Score = CalculateCvScore(jobId, entity, filePath);

        _dbcontext.Requests.Add(entity);
        await _dbcontext.SaveChangesAsync();

        Console.WriteLine($"Request created for JobId={jobId} UserId={userId}");

        Console.WriteLine("UploadCv completed");
        return Ok(new { message = "CV uploaded and request created successfully!" });
    }
    private string CalculateCvScore(Guid jobId, Request request, string filePath)
    {
        var appliedJob = _dbcontext.Jobs.FirstOrDefault(job => job.Id == jobId);
        if (appliedJob == null)
        {
            Console.WriteLine("Job not found for CV analysis.");
            return "0";
        }

        var jsonData = new
        {
            cvPath = filePath,
            jobDescription = appliedJob.Description,
            jobSkills = appliedJob.Skills
        };

        string jsonInput = JsonSerializer.Serialize(jsonData);
        string scriptPath = "/Users/dragoschisca/ProjectJob/ResumeChecker/main.py";

        var psi = new ProcessStartInfo
        {
            FileName = "python3",   
            Arguments = scriptPath,      
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    
        using var process = new Process { StartInfo = psi };
        process.Start();

        using (var sw = process.StandardInput)
        {
            sw.WriteLine(jsonInput);
            sw.Flush();
            sw.Close();
        }

        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();
        
        Console.WriteLine(output);

        process.WaitForExit();
    
        Console.WriteLine(jsonInput);
        
        if (!string.IsNullOrWhiteSpace(errors))
            Console.WriteLine($"Python error: {errors}");

        try
        {
            var result = JsonSerializer.Deserialize<JsonElement>(output);
            

            //Console.WriteLine(result.ToString());
            
            
            string score = result.GetProperty("score").GetRawText().Trim('"');

            Console.WriteLine($"CV score: {score}");

            // Setează score pe entity deja track-uit
            request.Score = score;

            return score;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON error: {ex.Message}");
            return "0";
        }
    }

    [HttpGet("GetCvScore/{userId}/{jobId}")]
    public IActionResult GetCvScoreForAppliedJob(Guid userId, Guid jobId)
    {
        var request = _dbcontext.Requests.Where(r => r.UserId == userId && r.JobId == jobId).FirstOrDefault();
    
        if (request == null)
        {
            return NotFound("Request not found");
        }
    
        return Ok(request.Score);
    }
}