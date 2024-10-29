using Fall2024_Assignment3_beboskus.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fall2024_Assignment3_beboskus.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movie {get; set; }
    public DbSet<Actor> Actor {get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);

        // Configure primary key for Movie and Actor
        modelBuilder.Entity<Movie>()
            .HasKey(m => m.Id);  // Use Id as the primary key instead of Title

        modelBuilder.Entity<Actor>()
            .HasKey(a => a.Id);  // Use Id as the primary key for Actor

        // Configure many-to-many relationship between Movies and Actors
        modelBuilder.Entity<Actor>()
            .HasMany(a => a.Movies)
            .WithMany(m => m.Actors)
            .UsingEntity(j => j.ToTable("MovieActors"));
    }

}

