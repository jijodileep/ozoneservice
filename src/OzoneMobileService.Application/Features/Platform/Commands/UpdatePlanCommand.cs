using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.Features.Platform.Commands;

public sealed record UpdatePlanCommand(Guid PlanId, UpdateSubscriptionPlanRequest Request)
    : ICommand<SubscriptionPlanResponse?>;
