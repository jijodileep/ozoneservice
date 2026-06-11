using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Features.Subscription.Queries;

public sealed record GetSubscriptionOptionsQuery(Guid TenantId) : IQuery<SubscriptionOptionsResponse?>;
