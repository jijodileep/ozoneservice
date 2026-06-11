using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Features.UpgradeRequests.Commands;

public sealed record RejectUpgradeRequestCommand(Guid RequestId, Guid ReviewerUserId, string? Reason)
    : ICommand<UpgradeRequestResponse?>;
