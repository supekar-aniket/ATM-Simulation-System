using ATM_Simulation_System.Areas.Identity.Data;
using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ATM_Simulation_System.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ATM_Simulation_SystemUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        // ApplicationUser -> Accounts (One-to-Many)
        builder.Entity<Account>()
            .HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // Account -> Cards (One-to-Many)
        builder.Entity<Card>()
            .HasOne(c => c.Account)
            .WithMany(a => a.Cards)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Cascade);


        // Account -> Transactions (One-to-Many)
        builder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);


        // DECIMAL CONFIGURATION (important for money)

        builder.Entity<Account>()
            .Property(a => a.Balance)
            .HasColumnType("decimal(12,2)");

        builder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(12,2)");

        builder.Entity<Transaction>()
            .Property(t => t.BalanceAfterTransaction)
            .HasColumnType("decimal(12,2)");
    }
}
