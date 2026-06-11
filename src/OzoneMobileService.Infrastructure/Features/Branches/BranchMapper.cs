using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Shared;

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

    internal static string? NormalizePhone(string? value) =>
        PhoneNormalizer.TryNormalizeOptional(value, out var phone) ? phone : null;

    internal static string? NormalizeGstNumber(string? value) =>
        GstinNormalizer.TryNormalizeOptional(value, out var gstin) ? gstin : null;
}
