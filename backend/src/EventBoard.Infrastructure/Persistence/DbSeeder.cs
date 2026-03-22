using EventBoard.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EventBoard.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, TimeProvider timeProvider, ILogger logger)
    {
        if (db.Events.Any())
        {
            logger.LogInformation("Database already seeded");
            return;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var events = new List<Event>
        {
            // Past events
            new() {
                Title = "Intro to .NET",
                Description = "A beginner workshop covering .NET fundamentals: the runtime, the SDK, and how to build your first web API from scratch.",
                StartsAt = now.AddDays(-30),
                Location = "Room 101, Tech Hub",
                MaxParticipants = 50,
                CurrentRegistrations = 47
            },
            new() {
                Title = "Docker Deep Dive",
                Description = "Hands-on containerization session. We covered Dockerfiles, multi-stage builds, docker-compose, and basic networking.",
                StartsAt = now.AddDays(-7),
                Location = "Online (Zoom)",
                MaxParticipants = 100,
                CurrentRegistrations = 100
            },

            // Upcoming — registration open, plenty of space
            new() {
                Title = "React + TypeScript Workshop",
                Description = "Build a production-ready component library using React 18 and TypeScript. We'll cover compound components, context patterns, and testing with Vitest.",
                StartsAt = now.AddDays(14),
                Location = "Conference Center A",
                MaxParticipants = 40,
                CurrentRegistrations = 12
            },

            // Upcoming — almost full (1 spot left)
            new() {
                Title = "System Design for Backend Engineers",
                Description = "Interactive whiteboard session. We'll design a URL shortener, a rate limiter, and a distributed cache — discussing trade-offs at each step.",
                StartsAt = now.AddDays(5),
                Location = "Boardroom 3",
                MaxParticipants = 20,
                CurrentRegistrations = 19
            },

            // Upcoming — almost full (3 spot left)
            new() {
                Title = "API Design Patterns (PRO)",
                Description = "Continuation of the API Design Patterns workshop. We'll design a URL shortener, a rate limiter, and a distributed cache — discussing trade-offs at each step. This is a paid workshop.",
                StartsAt = now.AddDays(6),
                Location = "Boardroom 3",
                MaxParticipants = 40,
                CurrentRegistrations = 37
            },

            // Upcoming — almost full (2 spot left)
            new() {
                Title = "API Design Patterns (PRO) - second edition",
                Description = "Continuation of the API Design Patterns workshop. We'll design a URL shortener, a rate limiter, and a distributed cache — discussing trade-offs at each step. This is a paid workshop.",
                StartsAt = now.AddDays(6),
                Location = "Boardroom 3",
                MaxParticipants = 40,
                CurrentRegistrations = 38
            },

            // Upcoming — fully booked
            new() {
                Title = "AI/ML Fundamentals",
                Description = "Practical introduction to machine learning with Python. No prior ML experience required — just bring a laptop and curiosity.",
                StartsAt = now.AddDays(3),
                Location = "Auditorium B",
                MaxParticipants = 60,
                CurrentRegistrations = 60
            },

            // Upcoming — registration window closed (starts in 10 hours, deadline already passed)
            new() {
                Title = "PostgreSQL Performance Tuning",
                Description = "Query optimization and indexing strategies for production databases. We'll use EXPLAIN ANALYZE and look at real slow-query logs.",
                StartsAt = now.AddHours(10),
                Location = "Lab 4",
                MaxParticipants = 25,
                CurrentRegistrations = 18
            },
        };

        logger.LogInformation("Seeding database with {EventCount} sample events", events.Count);

        db.Events.AddRange(events);
        await db.SaveChangesAsync();
    }
}
