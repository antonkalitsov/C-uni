using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MovieTracker.Application.Common;

namespace MovieTracker.Application.Movies.CreateMovie;

public static class CreateMovieEndpoint
{
    public static IEndpointConventionBuilder MapCreateMovieEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapPost("/api/movies", HandleAsync)
            .WithName("CreateMovie")
            .WithSummary("Creates a new movie")
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        CreateMovieRequest request,
        IValidator<CreateMovieRequest> validator,
        CreateMovieHandler handler,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await handler.HandleAsync(request, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/movies/{result.Value!.Id}", result.Value)
            : result.Error.ToProblemResult();
    }
}
