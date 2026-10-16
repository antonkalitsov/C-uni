using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Movies;

namespace MovieTracker.UnitTests.Mocks;

public sealed class MockMovieRepository(
    int totalCount,
    IReadOnlyList<Movie> movies) : IMovieRepository
{
    private readonly Dictionary<Guid, Movie> _movies = movies.ToDictionary(movie => movie.Id);

    public int RequestedSkip { get; private set; }
    public int RequestedTake { get; private set; }
    public List<Movie> AddedMovies { get; } = [];
    public List<Movie> UpdatedMovies { get; } = [];
    public List<Guid> DeletedIds { get; } = [];

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(totalCount);

    public Task<IReadOnlyList<Movie>> GetPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        RequestedSkip = skip;
        RequestedTake = take;
        return Task.FromResult(movies);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _movies.TryGetValue(id, out var movie);
        return Task.FromResult(movie);
    }

    public Task<bool> AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        if (!_movies.TryAdd(movie.Id, movie))
        {
            return Task.FromResult(false);
        }

        AddedMovies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        if (!_movies.ContainsKey(movie.Id))
        {
            return Task.FromResult(false);
        }

        _movies[movie.Id] = movie;
        UpdatedMovies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_movies.Remove(id))
        {
            return Task.FromResult(false);
        }

        DeletedIds.Add(id);
        return Task.FromResult(true);
    }
}
