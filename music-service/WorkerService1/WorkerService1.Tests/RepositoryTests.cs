using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WorkerService1.Data;
using WorkerService1.Entities;
using WorkerService1.Repositories;
using Xunit;

namespace WorkerService1.Tests;

public class RepositoryTests
{
    private static MusicDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<MusicDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new MusicDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_PersistsSong()
    {
        await using var context = CreateContext(nameof(CreateAsync_PersistsSong));
        var repository = new MusicRepository(context, NullLogger<MusicRepository>.Instance);

        var song = await repository.CreateAsync(new Song
        {
            Title = "Numb",
            Artist = "Linkin Park",
            Album = "Meteora",
            Genre = "Rock",
            DurationSeconds = 186
        }, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, song.Id);
        Assert.Single(context.Songs);
    }

    [Fact]
    public async Task UpdateAsync_WhenSongExists_UpdatesValues()
    {
        await using var context = CreateContext(nameof(UpdateAsync_WhenSongExists_UpdatesValues));
        var repository = new MusicRepository(context, NullLogger<MusicRepository>.Instance);
        var created = await repository.CreateAsync(new Song
        {
            Title = "Old Title",
            Artist = "Artist",
            Album = "Album",
            Genre = "Pop",
            DurationSeconds = 200
        }, CancellationToken.None);

        var updated = await repository.UpdateAsync(new Song
        {
            Id = created.Id,
            Title = "New Title",
            Artist = created.Artist,
            Album = created.Album,
            Genre = created.Genre,
            DurationSeconds = 210,
            ReleaseDate = new DateOnly(2025, 1, 1)
        }, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("New Title", updated!.Title);
        Assert.Equal(210, updated.DurationSeconds);
    }

    [Fact]
    public async Task DeleteAsync_RemovesSong()
    {
        await using var context = CreateContext(nameof(DeleteAsync_RemovesSong));
        var repository = new MusicRepository(context, NullLogger<MusicRepository>.Instance);
        var created = await repository.CreateAsync(new Song
        {
            Title = "Song",
            Artist = "Artist",
            Album = "Album",
            Genre = "Pop",
            DurationSeconds = 180
        }, CancellationToken.None);

        var deleted = await repository.DeleteAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Empty(context.Songs);
    }
}

