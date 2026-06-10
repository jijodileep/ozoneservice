namespace OzoneMobileService.Application.DTOs.Notifications;

public sealed record NotificationResponse(
    Guid Id,
    Guid? TenantId,
    string Title,
    string Message,
    string NotificationType,
    bool IsRead,
    DateTime CreatedAt);
