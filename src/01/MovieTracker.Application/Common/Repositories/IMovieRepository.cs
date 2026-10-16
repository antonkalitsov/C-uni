using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Common.Repositories;

public interface IMovieRepository
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Movie>> GetPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <returns>false if a movie with the same id already exists.</returns>
    Task<bool> AddAsync(Movie movie, CancellationToken cancellationToken = default);

    /// <returns>false if no movie with the given id exists.</returns>
    Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default);

    /// <returns>false if no movie with the given id exists.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
