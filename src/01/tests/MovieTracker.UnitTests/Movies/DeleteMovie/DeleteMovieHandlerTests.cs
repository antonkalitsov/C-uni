using MovieTracker.Application.Movies.DeleteMovie;
using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;
using MovieTracker.UnitTests.Mocks;

namespace MovieTracker.UnitTests.Movies.DeleteMovie;

[TestFixture]
public sealed class DeleteMovieHandlerTests : BaseUnitTest
{
    [Test]
    public async Task HandleAsync_WithExistingId_RemovesTheMovie()
    {
        var movie = Movie.Create(
            Guid.NewGuid(),
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            MovieGenre.Drama,
            2016,
            "/images/posters/movie.svg").Value!;
        var repository = new MockMovieRepository(totalCount: 1, [movie]);
        var handler = new DeleteMovieHandler(repository);

        var result = await handler.HandleAsync(movie.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(repository.DeletedIds, Is.EqualTo(new[] { movie.Id }));
        });
    }

    [Test]
    public async Task HandleAsync_WithUnknownId_ReturnsNotFound()
    {
        var repository = new MockMovieRepository(totalCount: 0, []);
        var handler = new DeleteMovieHandler(repository);

        var result = await handler.HandleAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(Error.NotFound));
        });
    }
}
