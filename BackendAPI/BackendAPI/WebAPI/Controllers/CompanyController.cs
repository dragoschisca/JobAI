using BackendAPI.Domain.Entites;
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
    
    [HttpGet("GetCompanyById/{id}")]
    public IActionResult GetCompanyById(Guid id)
    {
        var company = dbcontext.Companies.Find(id);
        
        if(company == null)  return NotFound();
        
        return Ok(company);
    }

    [HttpGet("{name}")]
    public IActionResult GetCompanyByName(string name)
    {
        return Ok(dbcontext.Companies.Find(name));
    }

    [HttpGet("GetCompanyJobsById/{id}")]
    public IActionResult GetCompanyJobsById(Guid Id)
    {
        return Ok(dbcontext.Jobs.Where(x => x.CompanyId == Id).ToList());
    }

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyDto addCompanyDto)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState);
        }
        
        var company = new Company()
        {
            Email = addCompanyDto.Email,
            Password = addCompanyDto.Password,
            Role = UserRole.Company,
            CompanyName = addCompanyDto.CompanyName,
            City = addCompanyDto.City,
            OfficeAddress = addCompanyDto.OfficeAddress,
            OfficePhone = addCompanyDto.OfficePhone,
            JobIds = null,
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