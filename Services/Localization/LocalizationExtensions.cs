namespace Moonatna.Services.Localization;

public static class LocalizationExtensions
{
    // Falls back to the key itself so missing entries are visible during development.
    public static string T(this IReadOnlyDictionary<string, string> strings, string key)
        => strings.TryGetValue(key, out var value) ? value : key;
}
