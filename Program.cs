using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ATM_Simulation_System.Areas.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using ATM_Simulation_System.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ATM_Simulation_SystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ATM_Simulation_SystemContext") ?? throw new InvalidOperationException("Connection string 'ATM_Simulation_SystemContext' not found.")));
var connectionString = builder.Configuration.GetConnectionString("DbContextConnection") ?? throw new InvalidOperationException("Connection string 'DbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ATM_Simulation_SystemUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.UseDeveloperExceptionPage();

app.Run();
