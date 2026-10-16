namespace MovieTracker.Domain.Movies;

public static class MovieGenreParser
{
    public static bool TryParse(string? value, out MovieGenre genre)
    {
        genre = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        foreach (var name in Enum.GetNames<MovieGenre>())
        {
            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                genre = Enum.Parse<MovieGenre>(name);
                return true;
            }
        }

        return false;
    }
}
