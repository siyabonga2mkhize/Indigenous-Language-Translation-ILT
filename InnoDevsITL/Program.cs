using InnoDevsITL.Data;
using InnoDevsITL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext < InnoDbContext> (options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
options.Password.RequireNonAlphanumeric = false;
options.Password.RequiredLength = 8;
options.Password.RequireUppercase = false;
options.Password.RequireLowercase = false;
options.User.RequireUniqueEmail = true;
options.SignIn.RequireConfirmedAccount = false;
options.SignIn.RequireConfirmedEmail = false;
options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<InnoDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

//Seeding for login in different roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Student", "Admin" };

    foreach (var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();//Added for authentication
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
