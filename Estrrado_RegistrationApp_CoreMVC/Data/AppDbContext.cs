using Microsoft.EntityFrameworkCore;
using Estrrado_RegistrationApp_CoreMVC.Models;

namespace Estrrado_RegistrationApp_CoreMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
    }
}

