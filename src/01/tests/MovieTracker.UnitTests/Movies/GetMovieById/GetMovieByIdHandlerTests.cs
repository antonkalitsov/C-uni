using MovieTracker.Application.Movies.GetMovieById;
using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;
using MovieTracker.UnitTests.Mocks;

namespace MovieTracker.UnitTests.Movies.GetMovieById;

[TestFixture]
public sealed class GetMovieByIdHandlerTests : BaseUnitTest
{
    [Test]
    public async Task HandleAsync_WithExistingId_ReturnsTheMappedMovie()
    {
        var movie = Movie.Create(
            Guid.NewGuid(),
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            MovieGenre.Drama,
            2016,
            $"/images/posters/{Faker.Random.AlphaNumeric(10)}.svg").Value!;
        var repository = new MockMovieRepository(totalCount: 1, [movie]);
        var handler = new GetMovieByIdHandler(repository);

        var result = await handler.HandleAsync(new GetMovieByIdRequest(movie.Id), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Id, Is.EqualTo(movie.Id));
            Assert.That(result.Value!.Title, Is.EqualTo(movie.Title));
        });
    }

    [Test]
    public async Task HandleAsync_WithUnknownId_ReturnsNotFound()
    {
        var repository = new MockMovieRepository(totalCount: 0, []);
        var handler = new GetMovieByIdHandler(repository);

        var result = await handler.HandleAsync(new GetMovieByIdRequest(Guid.NewGuid()), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(Error.NotFound));
        });
    }
}
