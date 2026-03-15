using WorkerService1.Entities;

namespace WorkerService1.Contracts;

public static class SongMappingExtensions
{
    public static SongResponse ToResponse(this Song song)
    {
        return new SongResponse
        {
            Id = song.Id,
            Title = song.Title,
            Artist = song.Artist,
            Album = song.Album,
            Genre = song.Genre,
            DurationSeconds = song.DurationSeconds,
            ReleaseDate = song.ReleaseDate,
            CreatedAtUtc = song.CreatedAtUtc
        };
    }

    public static Song ToEntity(this SongUpsertRequest request, Guid? id = null)
    {
        return new Song
        {
            Id = id ?? Guid.Empty,
            Title = request.Title,
            Artist = request.Artist,
            Album = request.Album,
            Genre = request.Genre,
            DurationSeconds = request.DurationSeconds,
            ReleaseDate = request.ReleaseDate
        };
    }
}

