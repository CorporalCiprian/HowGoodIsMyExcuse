using HowGoodIsMyExcuse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HowGoodIsMyExcuse.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Excuse> Excuses => Set<Excuse>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User constraints
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).IsRequired().HasMaxLength(50);
            e.Property(u => u.Email).IsRequired().HasMaxLength(100);
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
        });

        // Excuse constraints
        modelBuilder.Entity<Excuse>(e =>
        {
            e.Property(ex => ex.Text).IsRequired().HasMaxLength(500);
            e.Property(ex => ex.CreatedAt).HasDefaultValueSql("now()");
        });

        // one vote per user per excuse
        modelBuilder.Entity<Vote>(e =>
        {
            e.HasIndex(v => new { v.ExcuseId, v.UserId }).IsUnique();
            e.Property(v => v.CreatedAt).HasDefaultValueSql("now()");
        });
    }
}