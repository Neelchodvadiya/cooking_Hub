using Cooking_Hub.Data;
using Cooking_Hub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<CookingHubContext>(options =>
	options.UseSqlServer(connectionString));
/*builder.Services.AddDefaultIdentity<CookingHubUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();*/
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<CookingHubUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
using (var scop = app.Services.CreateScope())
{
    var roleManager = scop.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

}

using (var scop = app.Services.CreateScope())
{
    var userManager = scop.ServiceProvider.GetRequiredService<UserManager<CookingHubUser>>();
    string email = "admin@gmail.com";
    string password = "Test1234.";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new CookingHubUser();
        user.Email = email;
        user.UserName = email;
        user.EmailConfirmed = true;
        user.IsActive = true;
        user.CreatedAt = DateTime.Now;

        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
    }
}

app.Run();


/*[IsActive]
      ,[ImageFilePath]
      ,[Gender]*/