using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieTracker.Web.Models.Movies;

namespace MovieTracker.Web.Services.Movies;

public sealed class MovieApiClient(HttpClient httpClient)
{
    public async Task<MovieListResponseModel> GetMoviesAsync(int page = 1, int pageSize = 24, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/movies?page={page}&pageSize={pageSize}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieListResponseModel>(cancellationToken)
            ?? throw new InvalidOperationException("The movie API returned an empty response.");
    }

    /// <returns>null if the movie does not exist.</returns>
    public async Task<MovieDetailsModel?> GetMovieAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/movies/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieDetailsModel>(cancellationToken);
    }

    public async Task<MovieSaveResult> CreateMovieAsync(MovieFormModel movie, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/movies", movie, cancellationToken);
        return await ReadSaveResultAsync(response, cancellationToken);
    }

    public async Task<MovieSaveResult> UpdateMovieAsync(Guid id, MovieFormModel movie, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/movies/{id}", movie, cancellationToken);
        return await ReadSaveResultAsync(response, cancellationToken);
    }

    /// <returns>false if the movie does not exist.</returns>
    public async Task<bool> DeleteMovieAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/movies/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    private static async Task<MovieSaveResult> ReadSaveResultAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var movie = await response.Content.ReadFromJsonAsync<MovieDetailsModel>(cancellationToken)
                ?? throw new InvalidOperationException("The movie API returned an empty response.");
            return MovieSaveResult.Success(movie);
        }

        if (response.StatusCode is not (HttpStatusCode.BadRequest or HttpStatusCode.NotFound))
        {
            response.EnsureSuccessStatusCode();
        }

        return MovieSaveResult.Failure(await ReadProblemErrorsAsync(response, cancellationToken));
    }

    // Reads the messages from a ValidationProblem ("errors") or a Problem ("detail") response.
    private static async Task<IReadOnlyList<string>> ReadProblemErrorsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
        {
            return errors.EnumerateObject()
                .SelectMany(field => field.Value.EnumerateArray())
                .Select(message => message.GetString() ?? string.Empty)
                .Where(message => message.Length > 0)
                .ToArray();
        }

        if (root.TryGetProperty("detail", out var detail) && detail.GetString() is { Length: > 0 } message)
        {
            return [message];
        }

        return ["The movie could not be saved."];
    }
}
