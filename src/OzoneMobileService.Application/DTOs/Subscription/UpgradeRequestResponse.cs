namespace OzoneMobileService.Application.DTOs.Subscription;

public sealed record UpgradeRequestResponse(
    Guid Id,
    Guid TenantId,
    string TenantName,
    string CurrentPlanName,
    string RequestedPlanName,
    decimal RequestedPlanPrice,
    string Status,
    DateTime RequestedAt,
    DateTime? ReviewedAt,
    string? RejectionReason,
    Guid? InvoiceId,
    string? InvoiceNumber);

public sealed record RejectUpgradeRequestRequest(string? Reason);
