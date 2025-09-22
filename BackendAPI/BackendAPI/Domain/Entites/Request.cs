using System.ComponentModel.DataAnnotations;

namespace JobAPI.Domain.Entites;

public class Request
{
    [Key]
    public Guid Id { get; init; }
    public Guid JobId { get; set; }
    public Guid UserId { get; set; }
    public Status Status { get; set; }
}

public enum Status
{
    Rejected,
    Loading,
    Accepted,
    OnStayding
}