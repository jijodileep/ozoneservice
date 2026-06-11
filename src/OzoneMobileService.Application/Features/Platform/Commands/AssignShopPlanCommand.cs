using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Platform.Commands;

public sealed record AssignShopPlanCommand(Guid TenantId, Guid PlanId) : ICommand<bool>;
