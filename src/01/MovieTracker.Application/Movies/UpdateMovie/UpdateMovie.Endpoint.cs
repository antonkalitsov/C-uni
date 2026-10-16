using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MovieTracker.Application.Common;

namespace MovieTracker.Application.Movies.UpdateMovie;

public static class UpdateMovieEndpoint
{
    public static IEndpointConventionBuilder MapUpdateMovieEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapPut("/api/movies/{id:guid}", HandleAsync)
            .WithName("UpdateMovie")
            .WithSummary("Updates an existing movie")
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateMovieRequest request,
        IValidator<UpdateMovieRequest> validator,
        UpdateMovieHandler handler,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await handler.HandleAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemResult();
    }
}
