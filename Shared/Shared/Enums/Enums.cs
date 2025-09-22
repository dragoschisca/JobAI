namespace Shared.DTOs
{
    public enum UserRole
    {
        Admin,
        Company,
        Candidat
    }
    public enum WorkTime
    {
        FullTime,
        PartTime,
        FlexibleTime,
        InTurns
    }

    public enum Experience
    {
        NoExperience,
        SmallExperience,
        MediumExperience,
        LargeExperience
    }

    public enum Location
    {
        Remote,
        Local,
        Hybrid
    }
    public enum Status
    {
        Rejected,
        Loading,
        Accepted,
        OnStayding
    }
}