using WorkerService1.Entities;

namespace WorkerService1.Repositories;

public interface IMusicRepository
{
    Task<IReadOnlyCollection<Song>> GetSongsAsync(string? query, string? genre, string? artist, CancellationToken cancellationToken);
    Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Song> CreateAsync(Song song, CancellationToken cancellationToken);
    Task<Song?> UpdateAsync(Song song, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Song>> SearchAsync(string term, CancellationToken cancellationToken);
}

