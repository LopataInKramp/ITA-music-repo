using Microsoft.EntityFrameworkCore;
using WorkerService1.Data;
using WorkerService1.Entities;

namespace WorkerService1.Repositories;

public class MusicRepository(MusicDbContext context, ILogger<MusicRepository> logger) : IMusicRepository
{
    public async Task<IReadOnlyCollection<Song>> GetSongsAsync(string? query, string? genre, string? artist, CancellationToken cancellationToken)
    {
        var songsQuery = context.Songs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            songsQuery = songsQuery.Where(song => song.Title.Contains(query) || song.Album.Contains(query));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            songsQuery = songsQuery.Where(song => song.Genre == genre);
        }

        if (!string.IsNullOrWhiteSpace(artist))
        {
            songsQuery = songsQuery.Where(song => song.Artist == artist);
        }

        return await songsQuery
            .OrderBy(song => song.Title)
            .ToListAsync(cancellationToken);
    }

    public Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Songs.FirstOrDefaultAsync(song => song.Id == id, cancellationToken);

    public async Task<Song> CreateAsync(Song song, CancellationToken cancellationToken)
    {
        song.Id = Guid.NewGuid();
        song.CreatedAtUtc = DateTime.UtcNow;

        context.Songs.Add(song);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Song {SongId} created", song.Id);
        return song;
    }

    public async Task<Song?> UpdateAsync(Song song, CancellationToken cancellationToken)
    {
        var existingSong = await context.Songs.FirstOrDefaultAsync(item => item.Id == song.Id, cancellationToken);
        if (existingSong is null)
        {
            return null;
        }

        existingSong.Title = song.Title;
        existingSong.Artist = song.Artist;
        existingSong.Album = song.Album;
        existingSong.Genre = song.Genre;
        existingSong.DurationSeconds = song.DurationSeconds;
        existingSong.ReleaseDate = song.ReleaseDate;

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Song {SongId} updated", song.Id);
        return existingSong;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var song = await context.Songs.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (song is null)
        {
            return false;
        }

        context.Songs.Remove(song);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Song {SongId} deleted", id);
        return true;
    }

    public async Task<IReadOnlyCollection<Song>> SearchAsync(string term, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return [];
        }

        return await context.Songs
            .Where(song => song.Title.Contains(term) || song.Artist.Contains(term) || song.Album.Contains(term))
            .OrderBy(song => song.Title)
            .ToListAsync(cancellationToken);
    }
}

