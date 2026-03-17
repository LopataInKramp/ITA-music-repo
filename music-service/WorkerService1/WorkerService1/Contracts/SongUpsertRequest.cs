using System.ComponentModel.DataAnnotations;

namespace WorkerService1.Contracts;

public class SongUpsertRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Artist { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Album { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Genre { get; set; } = string.Empty;

    [Range(1, 24 * 60 * 60)]
    public int DurationSeconds { get; set; }

    public DateOnly? ReleaseDate { get; set; }
}

