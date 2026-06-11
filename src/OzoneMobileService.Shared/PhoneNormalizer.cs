namespace OzoneMobileService.Shared;

public static class PhoneNormalizer
{
    public static bool TryNormalize(string? input, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var digits = new string(input.Where(char.IsDigit).ToArray());
        if (digits.Length == 12 && digits.StartsWith("91", StringComparison.Ordinal))
        {
            digits = digits[2..];
        }

        if (digits.Length != 10)
        {
            return false;
        }

        normalized = digits;
        return true;
    }

    public static bool TryNormalizeOptional(string? input, out string? normalized)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            normalized = null;
            return true;
        }

        if (TryNormalize(input, out var mobile))
        {
            normalized = mobile;
            return true;
        }

        normalized = null;
        return false;
    }
}
