using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Domain.Movies;

public static class MovieErrors
{
    public static Error IdRequired { get; } =
        new("Movies.IdRequired", "A movie must have an id.");

    public static Error TitleRequired { get; } =
        new("Movies.TitleRequired", "A movie must have a title.");

    public static Error DescriptionRequired { get; } =
        new("Movies.DescriptionRequired", "A movie must have a description.");

    public static Error ReleaseYearTooEarly { get; } =
        new("Movies.ReleaseYearTooEarly", "A movie's release year cannot be earlier than 1888.");

    public static Error PosterUrlRequired { get; } =
        new("Movies.PosterUrlRequired", "A movie must have a poster URL.");

    public static Error InvalidGenre { get; } =
        new("Movies.InvalidGenre", "The specified genre is not valid.");
}
