using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Branches.Commands;

public sealed record DeactivateBranchCommand(Guid BranchId, Guid UserId) : ICommand<bool>;
