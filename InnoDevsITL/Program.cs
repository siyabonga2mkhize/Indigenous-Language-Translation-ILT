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

builder.Services.AddScoped<InnoDbContext>();
builder.Services.AddScoped<InnoDevsITL.Data.Repositories.Interfaces.IPhraseRepository, InnoDevsITL.Data.Repositories.Implementations.PhraseRepository>();
builder.Services.AddScoped<InnoDevsITL.Data.Repositories.Interfaces.ITranslationRepository, InnoDevsITL.Data.Repositories.Implementations.TranslationRepository>();
builder.Services.AddScoped<InnoDevsITL.Data.Repositories.Interfaces.ICategoryRepository, InnoDevsITL.Data.Repositories.Implementations.CategoryRepository>();
builder.Services.AddScoped<InnoDevsITL.Data.Repositories.Interfaces.IFavouriteRepository, InnoDevsITL.Data.Repositories.Implementations.FavouriteRepository>();
builder.Services.AddScoped<InnoDevsITL.Data.Repositories.Interfaces.ISubmissionRepository, InnoDevsITL.Data.Repositories.Implementations.SubmissionRepository>();

builder.Services.AddScoped<InnoDevsITL.Services.Interfaces.IPhraseService, InnoDevsITL.Services.Implementations.PhraseService>();
builder.Services.AddScoped<InnoDevsITL.Services.Interfaces.ITranslationService, InnoDevsITL.Services.Implementations.TranslationService>();
builder.Services.AddScoped<InnoDevsITL.Services.Interfaces.ICategoryService, InnoDevsITL.Services.Implementations.CategoryService>();
builder.Services.AddScoped<InnoDevsITL.Services.Interfaces.IFavouriteService, InnoDevsITL.Services.Implementations.FavouriteService>();
builder.Services.AddScoped<InnoDevsITL.Services.Interfaces.ISubmissionService, InnoDevsITL.Services.Implementations.SubmissionService>();

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
