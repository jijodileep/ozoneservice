using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Branches;

namespace OzoneMobileService.Application.Features.Branches.Commands;

public sealed record CreateBranchCommand(
    Guid TenantId,
    Guid UserId,
    string Code,
    string Name,
    string? Address,
    string? Phone,
    string? GstNumber) : ICommand<BranchResponse?>;
