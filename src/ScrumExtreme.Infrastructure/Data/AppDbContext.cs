using Microsoft.EntityFrameworkCore;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserStory> UserStories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API configuration goes here
    }
}
