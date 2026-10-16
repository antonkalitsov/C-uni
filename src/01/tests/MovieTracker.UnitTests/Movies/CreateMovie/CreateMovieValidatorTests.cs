using MovieTracker.Application.Movies.CreateMovie;
using MovieTracker.Domain.Movies;

namespace MovieTracker.UnitTests.Movies.CreateMovie;

[TestFixture]
public sealed class CreateMovieValidatorTests
{
    private CreateMovieValidator validator = null!;

    [SetUp]
    public void SetUp() => validator = new CreateMovieValidator();

    [Test]
    public async Task ValidateAsync_WithValidRequest_IsValid()
    {
        var request = new CreateMovieRequest(
            "Title",
            "Description",
            nameof(MovieGenre.Action),
            2020,
            "/images/posters/movie.svg");

        var result = await validator.ValidateAsync(request);

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("", "Description", "Action", 2020, "/poster.svg", nameof(CreateMovieRequest.Title))]
    [TestCase("Title", "", "Action", 2020, "/poster.svg", nameof(CreateMovieRequest.Description))]
    [TestCase("Title", "Description", "NotAGenre", 2020, "/poster.svg", nameof(CreateMovieRequest.Genre))]
    [TestCase("Title", "Description", "Action", 1887, "/poster.svg", nameof(CreateMovieRequest.ReleaseYear))]
    [TestCase("Title", "Description", "Action", 2020, "", nameof(CreateMovieRequest.PosterUrl))]
    public async Task ValidateAsync_WithInvalidField_HasExpectedError(
        string title,
        string description,
        string genre,
        int releaseYear,
        string posterUrl,
        string propertyName)
    {
        var request = new CreateMovieRequest(title, description, genre, releaseYear, posterUrl);

        var result = await validator.ValidateAsync(request);

        Assert.That(result.Errors.Select(error => error.PropertyName), Does.Contain(propertyName));
    }
}
