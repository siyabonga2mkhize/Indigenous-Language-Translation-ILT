using InnoDevsITL.Models;

namespace InnoDevsITL.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InnoDbContext>();

            // Seed Faculties
            if (!context.Faculties.Any())
            {
                context.Faculties.AddRange(
                    new Faculty { Name = "Faculty of Engineering" },
                    new Faculty { Name = "Faculty of Sciences" },
                    new Faculty { Name = "Faculty of Humanities" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Campuses
            if (!context.Campuses.Any())
            {
                context.Campuses.AddRange(
                    new Campus { Name = "Main Campus" },
                    new Campus { Name = "City Campus" },
                    new Campus { Name = "South Campus" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Greetings" },
                    new Category { Name = "Everyday Phrases" },
                    new Category { Name = "Academic" },
                    new Category { Name = "Business" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}