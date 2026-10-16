using FluentValidation;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.UpdateMovie;

public sealed class UpdateMovieValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(request => request.Genre)
            .Must(genre => MovieGenreParser.TryParse(genre, out _))
            .WithMessage($"Genre must be one of: {string.Join(", ", Enum.GetNames<MovieGenre>())}.");

        RuleFor(request => request.ReleaseYear)
            .GreaterThanOrEqualTo(1888)
            .WithMessage("Release year cannot be earlier than 1888.");

        RuleFor(request => request.PosterUrl)
            .NotEmpty()
            .WithMessage("Poster URL is required.");
    }
}
