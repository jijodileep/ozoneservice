using OzoneMobileService.Application.Common.Abstractions;

namespace OzoneMobileService.Application.Features.Notifications.Commands;

public sealed record MarkNotificationReadCommand(Guid NotificationId) : ICommand;
