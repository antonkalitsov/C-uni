using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.CreateMovie;

public sealed record CreateMovieRequest(
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);

public sealed class CreateMovieHandler(IMovieRepository movieRepository)
{
    public async Task<Result<MovieResponse>> HandleAsync(
        CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        if (!MovieGenreParser.TryParse(request.Genre, out var genre))
        {
            return MovieErrors.InvalidGenre;
        }

        var creationResult = Movie.Create(
            Guid.NewGuid(),
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
        await movieRepository.AddAsync(movie, cancellationToken);

        return Result<MovieResponse>.Success(movie.ToResponse());
    }
}
