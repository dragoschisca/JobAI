namespace Shared.DTOs;

public class CompanyDto : UserDto 
{
    public string CompanyName { get; set; }
    public string City { get; set; }
    public string OfficeAddress { get; set; }
    public string OfficePhone { get; set; }
    public ICollection<Guid> JobIds { get; set; } = new List<Guid>();
}