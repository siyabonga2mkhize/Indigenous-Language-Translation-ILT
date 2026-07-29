using InnoDevsITL.Data;
using InnoDevsITL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoDevsITL
{
    public class SeedDatabase
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting database seeding...");

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Setup services
            var services = new ServiceCollection();
            services.AddDbContext<InnoDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));
            
            services.AddIdentity<Users, IdentityRole>()
                .AddEntityFrameworkStores<InnoDbContext>()
                .AddDefaultTokenProviders();

            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<InnoDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("Database ensured.");

                // Seed Roles
                Console.WriteLine("Seeding roles...");
                string[] roleNames = { "Admin", "Student", "Teacher" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        Console.WriteLine($"Created role: {roleName}");
                    }
                }

                // Seed Admin
                Console.WriteLine("Seeding admin user...");
                string adminEmail = "admin@innodevs.com";
                string adminPassword = "Admin@123456";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var user = new Users
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Administrator",
                        PhysicalAddress = "123 Admin Street",
                        DateOfBirth = new DateTime(1980, 1, 1),
                        FacultyId = 1,
                        CampusId = 1,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                        Console.WriteLine("Created admin user");
                    }
                }

                // Seed Student
                Console.WriteLine("Seeding student user...");
                string studentEmail = "student@test.com";
                string studentPassword = "Student@123";
                var studentUser = await userManager.FindByEmailAsync(studentEmail);
                if (studentUser == null)
                {
                    var user = new Users
                    {
                        UserName = studentEmail,
                        Email = studentEmail,
                        FirstName = "Test",
                        LastName = "Student",
                        PhysicalAddress = "456 Student Avenue",
                        DateOfBirth = new DateTime(2000, 5, 15),
                        FacultyId = 1,
                        CampusId = 1,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, studentPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Student");
                        Console.WriteLine("Created student user");
                    }
                }

                // Seed Teacher
                Console.WriteLine("Seeding teacher user...");
                string teacherEmail = "teacher@test.com";
                string teacherPassword = "Teacher@123";
                var teacherUser = await userManager.FindByEmailAsync(teacherEmail);
                if (teacherUser == null)
                {
                    var user = new Users
                    {
                        UserName = teacherEmail,
                        Email = teacherEmail,
                        FirstName = "Test",
                        LastName = "Teacher",
                        PhysicalAddress = "789 Teacher Lane",
                        DateOfBirth = new DateTime(1985, 8, 20),
                        FacultyId = 1,
                        CampusId = 1,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, teacherPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Teacher");
                        Console.WriteLine("Created teacher user");
                    }
                }

                // Seed Faculties
                Console.WriteLine("Seeding faculties...");
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
                    Console.WriteLine("Added faculties");
                }

                // Seed Campuses
                Console.WriteLine("Seeding campuses...");
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
                    Console.WriteLine("Added campuses");
                }

                // Seed Categories
                Console.WriteLine("Seeding categories...");
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
                    Console.WriteLine("Added categories");
                }

                // Seed Phrases and Translations
                Console.WriteLine("Seeding phrases and translations...");
                if (!context.Phrases.Any())
                {
                    var greetingsCat = await context.Categories.FirstAsync(c => c.Name == "Greetings");
                    var everydayCat = await context.Categories.FirstAsync(c => c.Name == "Everyday Phrases");

                    var phrases = new List<Phrase>
                    {
                        new Phrase
                        {
                            EnglishText = "Hello",
                            Language = "Zulu",
                            Transcription = "Sawubona",
                            IsActive = true,
                            CategoryId = greetingsCat.Id,
                            Translations = new List<Translation>
                            {
                                new Translation { Text = "Sawubona", Language = "Zulu", IsApproved = true },
                                new Translation { Text = "Molo", Language = "Xhosa", IsApproved = true },
                                new Translation { Text = "Hallo", Language = "Afrikaans", IsApproved = true }
                            }
                        },
                        new Phrase
                        {
                            EnglishText = "How are you?",
                            Language = "Zulu",
                            Transcription = "Unjani?",
                            IsActive = true,
                            CategoryId = greetingsCat.Id,
                            Translations = new List<Translation>
                            {
                                new Translation { Text = "Unjani?", Language = "Zulu", IsApproved = true },
                                new Translation { Text = "Uphi?", Language = "Xhosa", IsApproved = true },
                                new Translation { Text = "Hoe gaan dit?", Language = "Afrikaans", IsApproved = true }
                            }
                        },
                        new Phrase
                        {
                            EnglishText = "Good morning",
                            Language = "Zulu",
                            Transcription = "Sawubona ekuseni",
                            IsActive = true,
                            CategoryId = greetingsCat.Id,
                            Translations = new List<Translation>
                            {
                                new Translation { Text = "Sawubona ekuseni", Language = "Zulu", IsApproved = true },
                                new Translation { Text = "Molo kusasa", Language = "Xhosa", IsApproved = true },
                                new Translation { Text = "Goeie more", Language = "Afrikaans", IsApproved = true }
                            }
                        },
                        new Phrase
                        {
                            EnglishText = "Thank you",
                            Language = "Zulu",
                            Transcription = "Ngiyabonga",
                            IsActive = true,
                            CategoryId = everydayCat.Id,
                            Translations = new List<Translation>
                            {
                                new Translation { Text = "Ngiyabonga", Language = "Zulu", IsApproved = true },
                                new Translation { Text = "Enkosi", Language = "Xhosa", IsApproved = true },
                                new Translation { Text = "Dankie", Language = "Afrikaans", IsApproved = true }
                            }
                        },
                        new Phrase
                        {
                            EnglishText = "Yes",
                            Language = "Zulu",
                            Transcription = "Yebo",
                            IsActive = true,
                            CategoryId = everydayCat.Id,
                            Translations = new List<Translation>
                            {
                                new Translation { Text = "Yebo", Language = "Zulu", IsApproved = true },
                                new Translation { Text = "Ewe", Language = "Xhosa", IsApproved = true },
                                new Translation { Text = "Ja", Language = "Afrikaans", IsApproved = true }
                            }
                        },
                        new Phrase
                        {
                            EnglishText = "No",
                            Language = "Zulu",
                            Transcription = "Cha",
                            IsActive = true,
                            CategoryId = everydayCat.Id,
                            Translations = new List<Translation>
                            {
                                new Translation { Text = "Cha", Language = "Zulu", IsApproved = true },
                                new Translation { Text = "Hayi", Language = "Xhosa", IsApproved = true },
                                new Translation { Text = "Nee", Language = "Afrikaans", IsApproved = true }
                            }
                        }
                    };

                    context.Phrases.AddRange(phrases);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Added {phrases.Count} phrases with translations");
                }

                // Seed Submissions
                Console.WriteLine("Seeding submissions...");
                if (!context.Submissions.Any())
                {
                    var student = await userManager.FindByEmailAsync("student@test.com");
                    var phrase = await context.Phrases.FirstOrDefaultAsync();

                    if (student != null && phrase != null)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                UserId = student.Id,
                                SubmittedText = "Ngiyaxolisa (I'm sorry)",
                                SubmittedAt = DateTime.UtcNow.AddDays(-2),
                                IsApproved = false,
                                PhraseId = phrase.Id
                            },
                            new Submission
                            {
                                UserId = student.Id,
                                SubmittedText = "Ngicela usizo (Please help me)",
                                SubmittedAt = DateTime.UtcNow.AddDays(-1),
                                IsApproved = false,
                                PhraseId = phrase.Id
                            }
                        };

                        context.Submissions.AddRange(submissions);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Added {submissions.Count} submissions");
                    }
                }

                Console.WriteLine("✅ Database seeding completed successfully!");
                Console.WriteLine("\nTest Users:");
                Console.WriteLine("  Admin: admin@innodevs.com / Admin@123456");
                Console.WriteLine("  Student: student@test.com / Student@123");
                Console.WriteLine("  Teacher: teacher@test.com / Teacher@123");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error seeding database: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}