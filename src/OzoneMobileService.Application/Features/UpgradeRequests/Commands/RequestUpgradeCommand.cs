using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Features.UpgradeRequests.Commands;

public sealed record RequestUpgradeCommand(Guid TenantId, Guid PlanId)
    : ICommand<UpgradeRequestResponse?>;
