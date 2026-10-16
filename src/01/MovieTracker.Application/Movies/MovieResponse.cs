namespace MovieTracker.Application.Movies;

public sealed record MovieResponse(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);
