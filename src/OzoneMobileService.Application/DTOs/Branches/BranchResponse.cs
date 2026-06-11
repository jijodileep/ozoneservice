namespace OzoneMobileService.Application.DTOs.Branches;

public sealed record BranchResponse(
    Guid Id,
    string Code,
    string Name,
    string? Address,
    string? Phone,
    string? GstNumber,
    bool IsActive,
    Guid TenantId);
