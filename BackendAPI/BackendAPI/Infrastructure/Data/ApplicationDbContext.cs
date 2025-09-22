using JobAPI.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace JobAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Candidat> Candidats { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Request>  Requests { get; set; }
}