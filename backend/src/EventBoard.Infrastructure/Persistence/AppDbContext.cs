using EventBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            e.Property(x => x.Location).HasMaxLength(300).IsRequired();
            e.Property(x => x.MaxParticipants).IsRequired();
            e.Property(x => x.CurrentRegistrations).IsRequired().HasDefaultValue(0);
            e.HasIndex(x => x.StartsAt);
        });

        modelBuilder.Entity<Registration>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.Name).HasMaxLength(200).IsRequired();
            r.Property(x => x.Email).HasMaxLength(320).IsRequired();
            r.Property(x => x.RegisteredAt).IsRequired();

            // Database-level guard: one email per event
            r.HasIndex(x => new { x.EventId, x.Email }).IsUnique();

            r.HasOne(x => x.Event)
             .WithMany(x => x.Registrations)
             .HasForeignKey(x => x.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
