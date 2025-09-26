using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DTOs;

public class CandidatDto : UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CvPath { get; set; }

    public string AboutMe { get; set; }
    
}