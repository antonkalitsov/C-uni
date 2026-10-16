namespace MovieTracker.Web.Models.Movies;

public sealed record MovieSaveResult(MovieDetailsModel? Movie, IReadOnlyList<string> Errors)
{
    public bool IsSuccess => Movie is not null;

    public static MovieSaveResult Success(MovieDetailsModel movie) => new(movie, []);

    public static MovieSaveResult Failure(IReadOnlyList<string> errors) => new(null, errors);
}
