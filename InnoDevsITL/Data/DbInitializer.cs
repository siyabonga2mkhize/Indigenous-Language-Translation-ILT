using InnoDevsITL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InnoDevsITL.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            
            var context = services.GetRequiredService<InnoDbContext>();
            var userManager = services.GetRequiredService<UserManager<Users>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                // Apply pending migrations
                logger.LogInformation("🔄 Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("✅ Migrations applied successfully");

                // ===== SEED ROLES =====
                logger.LogInformation("🔄 Seeding roles...");
                string[] roleNames = { "Admin", "Student", "Teacher" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        logger.LogInformation($"   ✅ Created role: {roleName}");
                    }
                }

                // ===== SEED FACULTIES =====
                logger.LogInformation("🔄 Seeding faculties...");
                if (!context.Faculties.Any())
                {
                    context.Faculties.AddRange(
                        new Faculty { Name = "Faculty of Engineering" },
                        new Faculty { Name = "Faculty of Sciences" },
                        new Faculty { Name = "Faculty of Humanities" },
                        new Faculty { Name = "Faculty of Commerce" },
                        new Faculty { Name = "Faculty of Education" }
                    );
                    await context.SaveChangesAsync();
                    logger.LogInformation("   ✅ Added 5 faculties");
                }

                // ===== SEED CAMPUSES =====
                logger.LogInformation("🔄 Seeding campuses...");
                if (!context.Campuses.Any())
                {
                    context.Campuses.AddRange(
                        new Campus { Name = "Main Campus" },
                        new Campus { Name = "City Campus" },
                        new Campus { Name = "South Campus" },
                        new Campus { Name = "North Campus" },
                        new Campus { Name = "Online Campus" }
                    );
                    await context.SaveChangesAsync();
                    logger.LogInformation("   ✅ Added 5 campuses");
                }

                // Get Faculty and Campus for users
                var faculty = await context.Faculties.FirstOrDefaultAsync();
                var campus = await context.Campuses.FirstOrDefaultAsync();

                if (faculty == null || campus == null)
                {
                    logger.LogError("❌ Cannot create users - Faculty or Campus not found");
                    return;
                }

                // ===== SEED USERS WITH PROPERLY HASHED PASSWORDS =====
                logger.LogInformation("🔄 Seeding users...");

                // Admin User
                var adminEmail = "admin@innodevs.com";
                var adminPassword = "Admin@123456";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                
                if (adminUser == null)
                {
                    var admin = new Users
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Administrator",
                        PhysicalAddress = "123 Admin Street",
                        DateOfBirth = new DateTime(1980, 1, 1),
                        FacultyId = faculty.Id,
                        CampusId = campus.Id,
                        EmailConfirmed = true
                    };
                    
                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        logger.LogInformation($"   ✅ Created admin: {adminEmail}");
                    }
                    else
                    {
                        logger.LogError($"   ❌ Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"   ℹ️  Admin user already exists: {adminEmail}");
                }

                // Student User
                var studentEmail = "student@test.com";
                var studentPassword = "Student@123456";
                var studentUser = await userManager.FindByEmailAsync(studentEmail);
                
                if (studentUser == null)
                {
                    var student = new Users
                    {
                        UserName = studentEmail,
                        Email = studentEmail,
                        FirstName = "Test",
                        LastName = "Student",
                        PhysicalAddress = "456 Student Avenue",
                        DateOfBirth = new DateTime(2000, 5, 15),
                        FacultyId = faculty.Id,
                        CampusId = campus.Id,
                        EmailConfirmed = true
                    };
                    
                    var result = await userManager.CreateAsync(student, studentPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(student, "Student");
                        logger.LogInformation($"   ✅ Created student: {studentEmail}");
                    }
                    else
                    {
                        logger.LogError($"   ❌ Failed to create student: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"   ℹ️  Student user already exists: {studentEmail}");
                }

                // Teacher User
                var teacherEmail = "teacher@test.com";
                var teacherPassword = "Teacher@123456";
                var teacherUser = await userManager.FindByEmailAsync(teacherEmail);
                
                if (teacherUser == null)
                {
                    var teacher = new Users
                    {
                        UserName = teacherEmail,
                        Email = teacherEmail,
                        FirstName = "Test",
                        LastName = "Teacher",
                        PhysicalAddress = "789 Teacher Lane",
                        DateOfBirth = new DateTime(1985, 8, 20),
                        FacultyId = faculty.Id,
                        CampusId = campus.Id,
                        EmailConfirmed = true
                    };
                    
                    var result = await userManager.CreateAsync(teacher, teacherPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(teacher, "Teacher");
                        logger.LogInformation($"   ✅ Created teacher: {teacherEmail}");
                    }
                    else
                    {
                        logger.LogError($"   ❌ Failed to create teacher: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"   ℹ️  Teacher user already exists: {teacherEmail}");
                }

                // ===== SEED CATEGORIES =====
                logger.LogInformation("🔄 Seeding categories...");
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name = "Greetings" },
                        new Category { Name = "Everyday Phrases" },
                        new Category { Name = "Academic" },
                        new Category { Name = "Business" },
                        new Category { Name = "Travel" },
                        new Category { Name = "Emergency" },
                        new Category { Name = "Food & Dining" },
                        new Category { Name = "Family & Relationships" }
                    );
                    await context.SaveChangesAsync();
                    logger.LogInformation("   ✅ Added 8 categories");
                }

                // ===== SEED PHRASES =====
                logger.LogInformation("🔄 Seeding phrases...");
                if (!context.Phrases.Any())
                {
                    var greetingsCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Greetings");
                    var everydayCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Everyday Phrases");

                    if (greetingsCategory != null && everydayCategory != null)
                    {
                        var phrases = new List<Phrase>
                        {
                            new Phrase
                            {
                                EnglishText = "Hello",
                                Language = "Zulu",
                                Transcription = "Sawubona",
                                IsActive = true,
                                CategoryId = greetingsCategory.Id
                            },
                            new Phrase
                            {
                                EnglishText = "How are you?",
                                Language = "Zulu",
                                Transcription = "Unjani?",
                                IsActive = true,
                                CategoryId = greetingsCategory.Id
                            },
                            new Phrase
                            {
                                EnglishText = "Good morning",
                                Language = "Zulu",
                                Transcription = "Sawubona ekuseni",
                                IsActive = true,
                                CategoryId = greetingsCategory.Id
                            },
                            new Phrase
                            {
                                EnglishText = "Thank you",
                                Language = "Zulu",
                                Transcription = "Ngiyabonga",
                                IsActive = true,
                                CategoryId = everydayCategory.Id
                            }
                        };

                        context.Phrases.AddRange(phrases);
                        await context.SaveChangesAsync();
                        logger.LogInformation($"   ✅ Added {phrases.Count} phrases");
                    }
                }

                logger.LogInformation("✅ Database initialization completed successfully!");
                logger.LogInformation("\n📋 Test Users Created:");
                logger.LogInformation("   Admin:   admin@innodevs.com / Admin@123456");
                logger.LogInformation("   Student: student@test.com / Student@123456");
                logger.LogInformation("   Teacher: teacher@test.com / Teacher@123456");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ An error occurred while seeding the database");
                throw;
            }
        }
    }
}
