using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.UpdateMovie;

public sealed record UpdateMovieRequest(
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);

public sealed class UpdateMovieHandler(IMovieRepository movieRepository)
{
    public async Task<Result<MovieResponse>> HandleAsync(
        Guid id,
        UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        if (!MovieGenreParser.TryParse(request.Genre, out var genre))
        {
            return MovieErrors.InvalidGenre;
        }

        var creationResult = Movie.Create(
            id,
            request.Title,
            request.Description,
            genre,
            request.ReleaseYear,
            request.PosterUrl);

        if (creationResult.IsFailure)
        {
            return Result<MovieResponse>.Failure(creationResult.Error);
        }

        var movie = creationResult.Value!;
        var updated = await movieRepository.UpdateAsync(movie, cancellationToken);

        return updated
            ? Result<MovieResponse>.Success(movie.ToResponse())
            : Result<MovieResponse>.Failure(Error.NotFound);
    }
}
