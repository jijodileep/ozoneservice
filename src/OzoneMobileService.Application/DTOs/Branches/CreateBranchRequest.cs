namespace OzoneMobileService.Application.DTOs.Branches;

public sealed record CreateBranchRequest(
    string Code,
    string Name,
    string? Address,
    string? Phone,
    string? GstNumber);
