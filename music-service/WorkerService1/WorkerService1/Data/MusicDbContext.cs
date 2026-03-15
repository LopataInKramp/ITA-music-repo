using Microsoft.EntityFrameworkCore;
using WorkerService1.Entities;

namespace WorkerService1.Data;

public class MusicDbContext(DbContextOptions<MusicDbContext> options) : DbContext(options)
{
    public DbSet<Song> Songs => Set<Song>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(song => song.Id);
            entity.Property(song => song.Title).HasMaxLength(200).IsRequired();
            entity.Property(song => song.Artist).HasMaxLength(200).IsRequired();
            entity.Property(song => song.Album).HasMaxLength(200).IsRequired();
            entity.Property(song => song.Genre).HasMaxLength(100).IsRequired();
            entity.HasIndex(song => new { song.Title, song.Artist });
        });
    }
}

