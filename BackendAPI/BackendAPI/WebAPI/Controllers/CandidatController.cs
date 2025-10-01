using BackendAPI.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using UserRole = Shared.DTOs.UserRole;

namespace BackendAPI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]

public class CandidatController : ControllerBase
{
    private readonly ApplicationDbContext dbcontext;

    public CandidatController(ApplicationDbContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }
    
    [HttpGet]
    public IActionResult GetAllCandidats()
    {
        return Ok(dbcontext.Candidats.ToList());
    }
    
    [HttpGet("{id}")]
    public IActionResult GetCandidatById(Guid id)
    {
        return Ok(dbcontext.Candidats.Find(id));
    }

    [HttpPost]
    public IActionResult CreateCandidat([FromBody] CandidatDto candidatDto)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState);
        }

        var candidat = new Candidat()
        {
            Email = candidatDto.Email,
            Password = candidatDto.Password,
            Role = UserRole.Candidat,
            FirstName = candidatDto.FirstName,
            LastName = candidatDto.LastName,
            CvPath = candidatDto.CvPath,
            AboutMe = candidatDto.AboutMe
        };
        
        dbcontext.Users.Add(candidat);
        dbcontext.SaveChanges();

        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCandidat(Guid id, [FromBody] CandidatDto candidatDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var candidat = dbcontext.Candidats.Find(id);
        
        candidat.FirstName = candidatDto.FirstName;
        candidat.LastName = candidatDto.LastName;
        candidat.CvPath = candidatDto.CvPath;
        candidat.AboutMe = candidatDto.AboutMe;
        
        dbcontext.SaveChanges();
        return Ok(candidat);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCandidat(Guid id)
    {
        var candidat = dbcontext.Candidats.Find(id);
        dbcontext.Candidats.Remove(candidat);
        dbcontext.SaveChanges();
        return Ok();
    }
    
}