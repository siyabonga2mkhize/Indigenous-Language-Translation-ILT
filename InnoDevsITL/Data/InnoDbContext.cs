using InnoDevsITL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Data
{
    public class InnoDbContext : IdentityDbContext<Users>

    {
        public InnoDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Campus> Campuses { get; set; }
    }
}
