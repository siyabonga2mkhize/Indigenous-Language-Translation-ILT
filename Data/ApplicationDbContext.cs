using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Models;


namespace PhraseBookk.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<UsageStat> UsageStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships to avoid cascade delete conflicts
            modelBuilder.Entity<Translation>()
                .HasOne(t => t.Phrase)
                .WithMany(p => p.Translations)
                .HasForeignKey(t => t.PhraseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Translation>()
                .HasOne(t => t.SubmittedBy)
                .WithMany(u => u.SubmittedTranslations)
                .HasForeignKey(t => t.SubmittedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Phrase)
                .WithMany(p => p.FavoritedByUsers)
                .HasForeignKey(f => f.PhraseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
