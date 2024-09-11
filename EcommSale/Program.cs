using EcommSale.Data;
using EcommSale.Models;
using EcommSale.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<UserManager<ApplicationUser>>();

// Currency thingy
builder.Services.AddHttpClient<CurrencyService>(client =>
{
    client.BaseAddress = new Uri("https://v6.exchangerate-api.com/v6/d859b2f38baeb9cf47d56825/latest/VND"); // Example URL, replace with your chosen API
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<CurrencyService>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    // You can change the password requirement here
    options.Password.RequireUppercase = false;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
    );
});
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Apply migrations

        // Seed data if necessary
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!roleManager.Roles.Any())
        {
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        // Create admin user if it does not exist
        if (!userManager.Users.Any(u => u.UserName == "admin"))
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@admin.com",
                EmailConfirmed = false
            };
            await userManager.CreateAsync(adminUser, "admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();