using MovieTracker.Application.Movies.UpdateMovie;
using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;
using MovieTracker.UnitTests.Mocks;

namespace MovieTracker.UnitTests.Movies.UpdateMovie;

[TestFixture]
public sealed class UpdateMovieHandlerTests : BaseUnitTest
{
    [Test]
    public async Task HandleAsync_WithExistingId_UpdatesAndReturnsTheMovie()
    {
        var existing = Movie.Create(
            Guid.NewGuid(),
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            MovieGenre.Drama,
            2016,
            "/images/posters/movie.svg").Value!;
        var repository = new MockMovieRepository(totalCount: 1, [existing]);
        var handler = new UpdateMovieHandler(repository);
        var request = new UpdateMovieRequest(
            "New Title",
            "New description",
            nameof(MovieGenre.Comedy),
            2021,
            "/images/posters/new.svg");

        var result = await handler.HandleAsync(existing.Id, request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Id, Is.EqualTo(existing.Id));
            Assert.That(result.Value!.Title, Is.EqualTo("New Title"));
            Assert.That(result.Value!.Genre, Is.EqualTo(nameof(MovieGenre.Comedy)));
            Assert.That(repository.UpdatedMovies, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task HandleAsync_WithUnknownId_ReturnsNotFound()
    {
        var repository = new MockMovieRepository(totalCount: 0, []);
        var handler = new UpdateMovieHandler(repository);
        var request = new UpdateMovieRequest(
            "Title",
            "Description",
            nameof(MovieGenre.Comedy),
            2021,
            "/images/posters/new.svg");

        var result = await handler.HandleAsync(Guid.NewGuid(), request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(Error.NotFound));
        });
    }

    [Test]
    public async Task HandleAsync_WithUnknownGenre_ReturnsInvalidGenre()
    {
        var existing = Movie.Create(
            Guid.NewGuid(),
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            MovieGenre.Drama,
            2016,
            "/images/posters/movie.svg").Value!;
        var repository = new MockMovieRepository(totalCount: 1, [existing]);
        var handler = new UpdateMovieHandler(repository);
        var request = new UpdateMovieRequest("Title", "Description", "NotAGenre", 2021, "/images/posters/new.svg");

        var result = await handler.HandleAsync(existing.Id, request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo("Movies.InvalidGenre"));
            Assert.That(repository.UpdatedMovies, Is.Empty);
        });
    }
}
