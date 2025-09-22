using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DTOs;

public class CandidatDto : UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CvPath { get; set; }

    public string? SkillsJson { get; set; }

    [NotMapped]
    public ICollection<string> Skills
    {
        get => string.IsNullOrEmpty(SkillsJson) 
            ? new List<string>() 
            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(SkillsJson)!;
        set => SkillsJson = System.Text.Json.JsonSerializer.Serialize(value);
    }
}