using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PhraseBookk.Data;
using PhraseBookk.Models;
using PhraseBookk.Services;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialPersonal("PhraseBookk Project");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{


    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAudioService, AzureTtsService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedDataAsync(context, userManager, roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ========== REDIRECT UNAUTHENTICATED USERS TO LOGIN ==========
app.Use(async (context, next) =>
{
    // Skip redirect for static files and Identity pages (Login/Register)
    if (context.Request.Path.StartsWithSegments("/lib") ||
        context.Request.Path.StartsWithSegments("/css") ||
        context.Request.Path.StartsWithSegments("/js") ||
        context.Request.Path.StartsWithSegments("/images") ||
        context.Request.Path.StartsWithSegments("/Identity/Account"))
    {
        await next();
        return;
    }

    // If user is not authenticated, redirect to Login page
    if (!context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }

    await next();
});
// ============================================================

// Redirect admin users to dashboard
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true && context.User.IsInRole("Administrator"))
    {
        var path = context.Request.Path.Value;
        if (path == "/" || path == "/Home/Index")
        {
            context.Response.Redirect("/PhrasesAdmin/Index");
            return;
        }
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();