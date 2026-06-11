namespace OzoneMobileService.Application.DTOs.Customers;

public sealed record CustomerHistoryItem(
    Guid Id,
    string Reference,
    string Status,
    string Type,
    DateTime OccurredAt);
