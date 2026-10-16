using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MovieTracker.Application.Common;

namespace MovieTracker.Application.Movies.GetMovieById;

public static class GetMovieByIdEndpoint
{
    public static IEndpointConventionBuilder MapGetMovieByIdEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapGet("/api/movies/{id:guid}", HandleAsync)
            .WithName("GetMovieById")
            .WithSummary("Gets a single movie by id")
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid id,
        GetMovieByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetMovieByIdRequest(id), cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemResult();
    }
}
