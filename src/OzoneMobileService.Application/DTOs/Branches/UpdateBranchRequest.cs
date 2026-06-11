namespace OzoneMobileService.Application.DTOs.Branches;

public sealed record UpdateBranchRequest(
    string Name,
    string? Address,
    string? Phone,
    string? GstNumber,
    bool IsActive);
