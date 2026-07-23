using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Models;


namespace PhraseBookk.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // ---- Seed Categories ----
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Registration", Description = "Academic registration", IsActive = true },
                    new Category { Name = "Accommodation", Description = "On-campus housing", IsActive = true },
                    new Category { Name = "Health Services", Description = "Campus clinic", IsActive = true },
                    new Category { Name = "Library", Description = "Library resources", IsActive = true },
                    new Category { Name = "Academic Support", Description = "Tutoring", IsActive = true }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // ---- Seed Admin Role ----
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            // ---- Seed Admin User ----
            var adminEmail = "admin@campus.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // ---- Seed Sample Phrases ----
            if (!context.Phrases.Any())
            {
                var reg = await context.Categories.FirstAsync(c => c.Name == "Registration");
                var acc = await context.Categories.FirstAsync(c => c.Name == "Accommodation");
                var health = await context.Categories.FirstAsync(c => c.Name == "Health Services");

                var phrases = new[]
                {
                    new Phrase { EnglishText = "Where is the registration office?", CategoryId = reg.Id, IsActive = true, CreatedDate = DateTime.Now },
                    new Phrase { EnglishText = "How do I apply for accommodation?", CategoryId = acc.Id, IsActive = true, CreatedDate = DateTime.Now },
                    new Phrase { EnglishText = "Where is the campus clinic?", CategoryId = health.Id, IsActive = true, CreatedDate = DateTime.Now }
                };
                await context.Phrases.AddRangeAsync(phrases);
                await context.SaveChangesAsync();

                // Get the saved phrases
                var p1 = await context.Phrases.FirstAsync(p => p.EnglishText == "Where is the registration office?");
                var p2 = await context.Phrases.FirstAsync(p => p.EnglishText == "How do I apply for accommodation?");
                var p3 = await context.Phrases.FirstAsync(p => p.EnglishText == "Where is the campus clinic?");

                // Add translations (isiZulu & Sesotho)
                var translations = new[]
                {
                    new Translation { PhraseId = p1.Id, Language = LanguageCode.zu, TranslatedText = "Iphi ihhovisi lokubhalisa?", Status = ContentStatus.Approved, CreatedDate = DateTime.Now },
                    new Translation { PhraseId = p1.Id, Language = LanguageCode.st, TranslatedText = "Ofisi ea ngoliso e kae?", Status = ContentStatus.Approved, CreatedDate = DateTime.Now },
                    new Translation { PhraseId = p2.Id, Language = LanguageCode.zu, TranslatedText = "Ngicela ukwenza isicelo sendawo yokuhlala?", Status = ContentStatus.Approved, CreatedDate = DateTime.Now },
                    new Translation { PhraseId = p3.Id, Language = LanguageCode.zu, TranslatedText = "Iphi ikliniki yasekhampasi?", Status = ContentStatus.Approved, CreatedDate = DateTime.Now }
                };
                await context.Translations.AddRangeAsync(translations);
                await context.SaveChangesAsync();
            }
        }
    }
}