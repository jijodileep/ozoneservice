namespace OzoneMobileService.Application.Exceptions;

public class PlanLimitException(string resource, int max)
    : Exception($"Plan limit reached (max {max} {resource})")
{
    public string Resource { get; } = resource;

    public int Max { get; } = max;
}
