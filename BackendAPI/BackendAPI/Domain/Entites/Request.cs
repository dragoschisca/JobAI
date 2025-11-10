using System.ComponentModel.DataAnnotations;
using Shared.DTOs;

namespace BackendAPI.Domain.Entites;

public class Request
{
    [Key]
    public Guid Id { get; init; }
    public Guid JobId { get; set; }
    public Guid UserId { get; set; }
    public Status Status { get; set; }
    public string Score { get; set; } = "0";
    public string CvFileName { get; set; } 
}
