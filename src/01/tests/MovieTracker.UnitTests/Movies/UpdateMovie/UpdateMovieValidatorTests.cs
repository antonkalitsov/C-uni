using MovieTracker.Application.Movies.UpdateMovie;
using MovieTracker.Domain.Movies;

namespace MovieTracker.UnitTests.Movies.UpdateMovie;

[TestFixture]
public sealed class UpdateMovieValidatorTests
{
    private UpdateMovieValidator validator = null!;

    [SetUp]
    public void SetUp() => validator = new UpdateMovieValidator();

    [Test]
    public async Task ValidateAsync_WithValidRequest_IsValid()
    {
        var request = new UpdateMovieRequest(
            "Title",
            "Description",
            nameof(MovieGenre.Action),
            2020,
            "/images/posters/movie.svg");

        var result = await validator.ValidateAsync(request);

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("", "Description", "Action", 2020, "/poster.svg", nameof(UpdateMovieRequest.Title))]
    [TestCase("Title", "", "Action", 2020, "/poster.svg", nameof(UpdateMovieRequest.Description))]
    [TestCase("Title", "Description", "NotAGenre", 2020, "/poster.svg", nameof(UpdateMovieRequest.Genre))]
    [TestCase("Title", "Description", "Action", 1887, "/poster.svg", nameof(UpdateMovieRequest.ReleaseYear))]
    [TestCase("Title", "Description", "Action", 2020, "", nameof(UpdateMovieRequest.PosterUrl))]
    public async Task ValidateAsync_WithInvalidField_HasExpectedError(
        string title,
        string description,
        string genre,
        int releaseYear,
        string posterUrl,
        string propertyName)
    {
        var request = new UpdateMovieRequest(title, description, genre, releaseYear, posterUrl);

        var result = await validator.ValidateAsync(request);

        Assert.That(result.Errors.Select(error => error.PropertyName), Does.Contain(propertyName));
    }
}
