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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Submission> Submissions { get; set; }
    }
}
