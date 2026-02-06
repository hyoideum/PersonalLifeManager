using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Habit>()
            .HasQueryFilter(h => !h.IsDeleted);
    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitEntry> HabitEntries { get; set; }
}