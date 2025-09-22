using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class RequestDto
{
        [Key]
        public Guid Id { get; init; }
        public Guid JobId { get; set; }
        public Guid UserId { get; set; }
        public Status Status { get; set; }

}