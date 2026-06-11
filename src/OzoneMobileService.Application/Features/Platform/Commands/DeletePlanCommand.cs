using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Platform.Commands;

public sealed record DeletePlanCommand(Guid PlanId) : ICommand<bool>;
