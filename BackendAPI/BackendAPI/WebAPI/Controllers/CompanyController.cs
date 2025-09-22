using JobAPI.Domain.Entites;
using JobAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace BackendAPI.WebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]

public class CompanyController : ControllerBase
{
    private readonly ApplicationDbContext dbcontext;

    public CompanyController(ApplicationDbContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    [HttpGet]
    public IActionResult GetAllCompanies()
    {
        return Ok(dbcontext.Companies.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetCompanyById(int id)
    {
        return Ok(dbcontext.Users.Find(id));
    }

    [HttpPost]
    public IActionResult AddCompany([FromBody] CompanyDto addCompanyDto)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState);
        }
        
        var jobs = dbcontext.Jobs
            .Where(j => addCompanyDto.JobIds.Contains(j.Id))
            .ToList();

        var company = new Company()
        {
            Email = addCompanyDto.Email,
            Password = addCompanyDto.Password,
            Role = UserRole.Company,
            CompanyName = addCompanyDto.CompanyName,
            City = addCompanyDto.City,
            OfficeAddress = addCompanyDto.OfficeAddress,
            OfficePhone = addCompanyDto.OfficePhone,
            JobIds = jobs.Select(j => j.Id).ToList()
        };

        dbcontext.Users.Add(company);
        dbcontext.SaveChanges();
        
        return Ok(company);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCompany(Guid id, [FromBody] CompanyDto updateCompanyDto)
    {
        var company = dbcontext.Companies.Find(id);
        
        company.CompanyName = updateCompanyDto.CompanyName;
        company.OfficeAddress = updateCompanyDto.OfficeAddress;
        company.OfficePhone = updateCompanyDto.OfficePhone;
        company.City = updateCompanyDto.City;
        company.JobIds = updateCompanyDto.JobIds;
        
        dbcontext.SaveChanges();
        return Ok(company);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCompany(Guid id)
    {
        var company = dbcontext.Companies.Find(id);
        dbcontext.Companies.Remove(company);
        dbcontext.SaveChanges();
        return Ok();
    }
}