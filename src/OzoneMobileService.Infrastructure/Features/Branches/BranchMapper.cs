using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.Branches;

internal static class BranchMapper
{
    internal static BranchResponse Map(Branch branch) =>
        new(
            branch.Id,
            branch.Code,
            branch.Name,
            branch.Address,
            branch.Phone,
            branch.GstNumber,
            branch.IsActive,
            branch.TenantId);

    internal static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
