using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ATM_Simulation_System.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'DbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ATM_Simulation_SystemUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ATM_Simulation_SystemUser>>();

    // Ensure roles exist
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Read admin details from appsettings.json
    var adminConfig = builder.Configuration.GetSection("AdminUser");
    string adminEmail = adminConfig["Email"]!;
    string adminPassword = adminConfig["Password"]!;
    string firstName = adminConfig["FirstName"]!;
    string lastName = adminConfig["LastName"]!;

    // Find existing admin
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        // Create new Admin
        adminUser = new ATM_Simulation_SystemUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            DateAndTime = DateTime.Now
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        // Update details if changed
        bool updateNeeded = false;

        if (adminUser.FirstName != firstName) { adminUser.FirstName = firstName; updateNeeded = true; }
        if (adminUser.LastName != lastName) { adminUser.LastName = lastName; updateNeeded = true; }

        if (updateNeeded)
        {
            await userManager.UpdateAsync(adminUser);
        }

        // Update password if different
        var passwordValid = await userManager.CheckPasswordAsync(adminUser, adminPassword);
        if (!passwordValid)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            await userManager.ResetPasswordAsync(adminUser, token, adminPassword);
        }

        // Ensure Admin role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
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

app.Run();
