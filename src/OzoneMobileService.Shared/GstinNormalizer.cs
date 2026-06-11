using System.Text.RegularExpressions;

namespace OzoneMobileService.Shared;

public static partial class GstinNormalizer
{
    [GeneratedRegex(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$", RegexOptions.CultureInvariant)]
    private static partial Regex GstinPattern();

    public static bool TryNormalizeOptional(string? input, out string? normalized)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            normalized = null;
            return true;
        }

        var value = input.Trim().ToUpperInvariant().Replace(" ", string.Empty);
        if (value.Length != 15 || !GstinPattern().IsMatch(value))
        {
            normalized = null;
            return false;
        }

        normalized = value;
        return true;
    }
}
