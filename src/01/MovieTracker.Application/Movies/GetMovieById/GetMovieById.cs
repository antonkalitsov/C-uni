using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.GetMovieById;

public sealed record GetMovieByIdRequest(Guid Id);

public sealed class GetMovieByIdHandler(IMovieRepository movieRepository)
{
    public async Task<Result<MovieResponse>> HandleAsync(
        GetMovieByIdRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.Id, cancellationToken);

        return movie is null
            ? Result<MovieResponse>.Failure(Error.NotFound)
            : Result<MovieResponse>.Success(movie.ToResponse());
    }
}
