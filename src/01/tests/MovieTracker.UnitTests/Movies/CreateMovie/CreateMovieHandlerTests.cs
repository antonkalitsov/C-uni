using MovieTracker.Application.Movies.CreateMovie;
using MovieTracker.Domain.Movies;
using MovieTracker.UnitTests.Mocks;

namespace MovieTracker.UnitTests.Movies.CreateMovie;

[TestFixture]
public sealed class CreateMovieHandlerTests : BaseUnitTest
{
    [Test]
    public async Task HandleAsync_WithValidRequest_AddsAndReturnsTheMovie()
    {
        var repository = new MockMovieRepository(totalCount: 0, []);
        var handler = new CreateMovieHandler(repository);
        var request = new CreateMovieRequest(
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            nameof(MovieGenre.Drama),
            2020,
            $"/images/posters/{Faker.Random.AlphaNumeric(10)}.svg");

        var result = await handler.HandleAsync(request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Value!.Title, Is.EqualTo(request.Title));
            Assert.That(result.Value!.Genre, Is.EqualTo(nameof(MovieGenre.Drama)));
            Assert.That(repository.AddedMovies, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task HandleAsync_WithUnknownGenre_ReturnsInvalidGenre()
    {
        var repository = new MockMovieRepository(totalCount: 0, []);
        var handler = new CreateMovieHandler(repository);
        var request = new CreateMovieRequest(
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            "NotAGenre",
            2020,
            "/images/posters/movie.svg");

        var result = await handler.HandleAsync(request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo("Movies.InvalidGenre"));
            Assert.That(repository.AddedMovies, Is.Empty);
        });
    }
}
