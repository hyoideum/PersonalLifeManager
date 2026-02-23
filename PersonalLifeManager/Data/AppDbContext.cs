using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Habit>()
            .HasQueryFilter(h => !h.IsDeleted);
        
        builder.Entity<HabitEntry>()
            .HasIndex(e => new { e.UserId, e.HabitId, e.Date })
            .IsUnique();
    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitEntry> HabitEntries { get; set; }
}