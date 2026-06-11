using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Platform;

namespace OzoneMobileService.Application.Features.Platform.Queries;

public sealed record GetPlansQuery : IQuery<IReadOnlyList<SubscriptionPlanResponse>>;
