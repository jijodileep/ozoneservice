using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Notifications;

namespace OzoneMobileService.Application.Features.Notifications.Queries;

public sealed record GetNotificationsQuery : IQuery<IReadOnlyList<NotificationResponse>>;
