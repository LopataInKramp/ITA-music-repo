namespace WorkerService1.Contracts;

public class SongResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

