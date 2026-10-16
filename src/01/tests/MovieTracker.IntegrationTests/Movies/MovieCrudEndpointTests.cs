using System.Net;
using System.Net.Http.Json;
using MovieTracker.Application.Movies;
using MovieTracker.Application.Movies.CreateMovie;
using MovieTracker.Application.Movies.UpdateMovie;
using MovieTracker.Domain.Movies;

namespace MovieTracker.IntegrationTests.Movies;

[TestFixture]
public sealed class MovieCrudEndpointTests : BaseIntegrationTest
{
    [Test]
    public async Task GetMovieById_WithKnownSeededId_ReturnsTheMovie()
    {
        var response = await Client.GetAsync("/api/movies/00000000-0000-0000-0000-000000000003");
        var payload = await response.Content.ReadFromJsonAsync<MovieResponse>();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload!.Title, Is.EqualTo("Arrival"));
        });
    }

    [Test]
    public async Task GetMovieById_WithUnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/movies/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateMovie_WithValidRequest_ReturnsCreatedAndIsThenRetrievable()
    {
        var request = new CreateMovieRequest(
            "Integration Test Movie",
            "A movie created during an integration test.",
            nameof(MovieGenre.Thriller),
            2022,
            "/images/posters/integration-test.svg");

        var createResponse = await Client.PostAsJsonAsync("/api/movies", request);
        var created = await createResponse.Content.ReadFromJsonAsync<MovieResponse>();

        var getResponse = await Client.GetAsync($"/api/movies/{created!.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<MovieResponse>();

        Assert.Multiple(() =>
        {
            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(createResponse.Headers.Location!.ToString(), Does.EndWith($"/api/movies/{created.Id}"));
            Assert.That(created.Title, Is.EqualTo(request.Title));
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(fetched!.Title, Is.EqualTo(request.Title));
        });
    }

    [Test]
    public async Task CreateMovie_WithInvalidRequest_ReturnsValidationProblem()
    {
        var request = new CreateMovieRequest(string.Empty, "Description", "NotAGenre", 1800, "");

        var response = await Client.PostAsJsonAsync("/api/movies", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateMovie_WithExistingMovie_ReturnsUpdatedMovie()
    {
        var createResponse = await Client.PostAsJsonAsync(
            "/api/movies",
            new CreateMovieRequest("Original Title", "Original description", nameof(MovieGenre.Action), 2010, "/images/posters/original.svg"));
        var created = await createResponse.Content.ReadFromJsonAsync<MovieResponse>();

        var updateRequest = new UpdateMovieRequest(
            "Updated Title",
            "Updated description",
            nameof(MovieGenre.Fantasy),
            2011,
            "/images/posters/updated.svg");
        var updateResponse = await Client.PutAsJsonAsync($"/api/movies/{created!.Id}", updateRequest);
        var updated = await updateResponse.Content.ReadFromJsonAsync<MovieResponse>();

        Assert.Multiple(() =>
        {
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(updated!.Id, Is.EqualTo(created.Id));
            Assert.That(updated.Title, Is.EqualTo("Updated Title"));
            Assert.That(updated.Genre, Is.EqualTo(nameof(MovieGenre.Fantasy)));
        });
    }

    [Test]
    public async Task UpdateMovie_WithUnknownId_ReturnsNotFound()
    {
        var updateRequest = new UpdateMovieRequest("Title", "Description", nameof(MovieGenre.Action), 2011, "/images/posters/updated.svg");

        var response = await Client.PutAsJsonAsync($"/api/movies/{Guid.NewGuid()}", updateRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteMovie_WithExistingMovie_RemovesItFromTheCatalogue()
    {
        var createResponse = await Client.PostAsJsonAsync(
            "/api/movies",
            new CreateMovieRequest("To Be Deleted", "Description", nameof(MovieGenre.Crime), 2015, "/images/posters/to-be-deleted.svg"));
        var created = await createResponse.Content.ReadFromJsonAsync<MovieResponse>();

        var deleteResponse = await Client.DeleteAsync($"/api/movies/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/movies/{created.Id}");

        Assert.Multiple(() =>
        {
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task DeleteMovie_WithUnknownId_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/movies/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
