using InnoDevsITL.Data;
using InnoDevsITL.Data.Repositories.Implementations;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Implementations;
using InnoDevsITL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<InnoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Configure Identity
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<InnoDbContext>()
.AddDefaultTokenProviders();

// Register Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPhraseService, PhraseService>();
builder.Services.AddScoped<ITranslationService, MockTranslationService>();
builder.Services.AddScoped<IFavouriteService, FavouriteService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
// Add these lines after the service registrations
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPhraseRepository, PhraseRepository>();
builder.Services.AddScoped<ITranslationRepository, TranslationRepository>();
builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<InnoDbContext>();
        var userManager = services.GetRequiredService<UserManager<Users>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed Roles
        string[] roleNames = { "Admin", "Student", "Teacher" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed Admin
        string adminEmail = "admin@innodevs.com";
        string adminPassword = "Admin@123456";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var user = new Users
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                PhysicalAddress = "Admin Address",
                DateOfBirth = DateTime.Now.AddYears(-30),
                FacultyId = 1,
                CampusId = 1,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

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
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();