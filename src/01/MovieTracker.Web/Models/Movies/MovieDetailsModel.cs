namespace MovieTracker.Web.Models.Movies;

public sealed record MovieDetailsModel(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);
