namespace MovieTracker.Web.Models.Movies;

public sealed class MovieFormModel
{
    public static readonly IReadOnlyList<string> Genres =
        ["Action", "Drama", "Comedy", "ScienceFiction", "Thriller", "Animation", "Crime", "Fantasy"];

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = Genres[0];
    public int ReleaseYear { get; set; } = DateTime.Today.Year;
    public string PosterUrl { get; set; } = string.Empty;

    public static MovieFormModel FromMovie(MovieDetailsModel movie) => new()
    {
        Title = movie.Title,
        Description = movie.Description,
        Genre = movie.Genre,
        ReleaseYear = movie.ReleaseYear,
        PosterUrl = movie.PosterUrl
    };
}
