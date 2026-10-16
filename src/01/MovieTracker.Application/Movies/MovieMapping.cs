using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies;

internal static class MovieMapping
{
    internal static MovieResponse ToResponse(this Movie movie) =>
        new(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.Genre.ToString(),
            movie.ReleaseYear,
            movie.PosterUrl);
}
