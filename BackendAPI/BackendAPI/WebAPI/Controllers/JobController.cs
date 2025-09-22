

using JobAPI.Domain.Entites;
using JobAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace BackendAPI.WebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]

public class JobController : ControllerBase
{
    private readonly ApplicationDbContext dbcontext;

    public JobController(ApplicationDbContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    [HttpGet]
    public IActionResult GetAllJobs()
    {
        return Ok(dbcontext.Jobs.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetJobById(Guid id)
    {
        return Ok(dbcontext.Jobs.Find(id));        
    }
    
    [HttpGet("{Category}")]
    public IActionResult GetJobByCategory(string Category)
    {
        return Ok(dbcontext.Jobs.Where(j => j.Category == Category).ToList());
    }

    [HttpPost]
    public IActionResult AddJob(JobDto addJobDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var job = new Job()
        {
            Title = addJobDto.Title,
            Description = addJobDto.Description,
            Skills = addJobDto.Skills,
            IsSalaryMentionated = addJobDto.IsSalaryMentioned,
            Salary = addJobDto.Salary,
            Category = addJobDto.Category,
            Experience = addJobDto.Experience,
            WorkTime = addJobDto.WorkTime,
            Location = addJobDto.Location,
        };
        
        dbcontext.Jobs.Add(job);
        dbcontext.SaveChanges();
        return Ok(job);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateJob(Guid id, JobDto updateJobDto)
    {
        var job = dbcontext.Jobs.Find(id);

        job.Title = updateJobDto.Title;
        job.Description = updateJobDto.Description;
        job.Skills = updateJobDto.Skills;
        job.IsSalaryMentionated = updateJobDto.IsSalaryMentioned;
        job.Salary = updateJobDto.Salary;
        job.Category = updateJobDto.Category;
        job.Experience = updateJobDto.Experience;
        job.WorkTime = updateJobDto.WorkTime;
        job.Location = updateJobDto.Location;
        
        dbcontext.SaveChanges();
        return Ok(job);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteJob(Guid id)
    {
        var job = dbcontext.Jobs.Find(id);
        dbcontext.Jobs.Remove(job);
        dbcontext.SaveChanges();
        
        return Ok();
    }
}