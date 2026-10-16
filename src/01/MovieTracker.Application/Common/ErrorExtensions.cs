using Microsoft.AspNetCore.Http;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Common;

public static class ErrorExtensions
{
    public static IResult ToProblemResult(this Error error) =>
        error == Error.NotFound
            ? Results.Problem(detail: error.Message, statusCode: StatusCodes.Status404NotFound)
            : Results.Problem(detail: error.Message, statusCode: StatusCodes.Status400BadRequest);
}
