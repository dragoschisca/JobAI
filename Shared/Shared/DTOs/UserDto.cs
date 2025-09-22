using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class UserDto
{
    [Key]
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}
