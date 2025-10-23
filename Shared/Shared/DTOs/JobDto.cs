namespace Shared.DTOs;

    public class JobDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
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
    } 