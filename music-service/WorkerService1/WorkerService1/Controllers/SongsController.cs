using Microsoft.AspNetCore.Mvc;
using WorkerService1.Contracts;
using WorkerService1.Repositories;

namespace WorkerService1.Controllers;

[ApiController]
[Route("api/songs")]
public class SongsController(IMusicRepository repository, ILogger<SongsController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SongResponse>>> GetSongs(
        [FromQuery] string? query,
        [FromQuery] string? genre,
        [FromQuery] string? artist,
        CancellationToken cancellationToken)
    {
        var songs = await repository.GetSongsAsync(query, genre, artist, cancellationToken);
        return Ok(songs.Select(song => song.ToResponse()).ToList());
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SongResponse>>> Search(
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {
        var songs = await repository.SearchAsync(term, cancellationToken);
        return Ok(songs.Select(song => song.ToResponse()).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SongResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var song = await repository.GetByIdAsync(id, cancellationToken);
        if (song is null)
        {
            return NotFound();
        }

        return Ok(song.ToResponse());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SongResponse>> Create([FromBody] SongUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdSong = await repository.CreateAsync(request.ToEntity(), cancellationToken);
        logger.LogInformation("Created song {SongId}", createdSong.Id);

        return CreatedAtAction(nameof(GetById), new { id = createdSong.Id }, createdSong.ToResponse());
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SongResponse>> Update(Guid id, [FromBody] SongUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedSong = await repository.UpdateAsync(request.ToEntity(id), cancellationToken);
        if (updatedSong is null)
        {
            return NotFound();
        }

        logger.LogInformation("Updated song {SongId}", id);
        return Ok(updatedSong.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        logger.LogInformation("Deleted song {SongId}", id);
        return NoContent();
    }
}

