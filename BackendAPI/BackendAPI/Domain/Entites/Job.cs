using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.DTOs;

namespace BackendAPI.Domain.Entites;

public class Job
{
    [Key]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; }
    public string Skills { get; set; }
    public bool IsSalaryMentionated { get; set; }
    public int? Salary { get; set; }
    public string Category { get; set; }
    public Experience Experience { get; set; }
    public WorkTime WorkTime { get; set; }
    public Location Location { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    
    public Guid CompanyId { get; set; }
   // [ForeignKey("CompanyId")]
    //public virtual Company Company { get; set; }
}
