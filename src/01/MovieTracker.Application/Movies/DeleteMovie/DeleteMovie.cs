using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.DeleteMovie;

public sealed class DeleteMovieHandler(IMovieRepository movieRepository)
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await movieRepository.DeleteAsync(id, cancellationToken);

        return deleted ? Result.Success() : Result.Failure(Error.NotFound);
    }
}
