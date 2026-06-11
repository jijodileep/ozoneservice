using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Branches;

namespace OzoneMobileService.Application.Features.Branches.Commands;

public sealed record UpdateBranchCommand(
    Guid BranchId,
    Guid UserId,
    string Name,
    string? Address,
    string? Phone,
    string? GstNumber,
    bool IsActive) : ICommand<BranchResponse?>;
