using System.Net;
using System.Net.Http.Json;
using WorkerService1.Contracts;
using Xunit;

namespace WorkerService1.Tests;

public class SongEndpointsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static SongUpsertRequest BuildSongRequest(string title = "Song A")
    {
        return new SongUpsertRequest
        {
            Title = title,
            Artist = "Artist A",
            Album = "Album A",
            Genre = "Rock",
            DurationSeconds = 210,
            ReleaseDate = new DateOnly(2024, 2, 1)
        };
    }

    [Fact]
    public async Task Post_CreateSong_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/songs", BuildSongRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<SongResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
    }

    [Fact]
    public async Task Get_AllSongs_ReturnsOk()
    {
        await _client.PostAsJsonAsync("/api/songs", BuildSongRequest("Song One"));

        var response = await _client.GetAsync("/api/songs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var songs = await response.Content.ReadFromJsonAsync<List<SongResponse>>();
        Assert.NotNull(songs);
        Assert.NotEmpty(songs!);
    }

    [Fact]
    public async Task Get_ById_ReturnsSong()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/songs", BuildSongRequest("ById Song"));
        var created = await createResponse.Content.ReadFromJsonAsync<SongResponse>();

        var response = await _client.GetAsync($"/api/songs/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var song = await response.Content.ReadFromJsonAsync<SongResponse>();
        Assert.Equal("ById Song", song!.Title);
    }

    [Fact]
    public async Task Put_UpdateSong_ReturnsUpdatedSong()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/songs", BuildSongRequest("Before Update"));
        var created = await createResponse.Content.ReadFromJsonAsync<SongResponse>();

        var updateRequest = BuildSongRequest("After Update");
        var response = await _client.PutAsJsonAsync($"/api/songs/{created!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<SongResponse>();
        Assert.Equal("After Update", updated!.Title);
    }

    [Fact]
    public async Task Delete_Song_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/songs", BuildSongRequest("Delete Me"));
        var created = await createResponse.Content.ReadFromJsonAsync<SongResponse>();

        var response = await _client.DeleteAsync($"/api/songs/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Get_Search_ReturnsMatches()
    {
        await _client.PostAsJsonAsync("/api/songs", BuildSongRequest("Searchable Song"));

        var response = await _client.GetAsync("/api/songs/search?term=Searchable");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var songs = await response.Content.ReadFromJsonAsync<List<SongResponse>>();
        Assert.NotNull(songs);
        Assert.Contains(songs!, song => song.Title == "Searchable Song");
    }
}

