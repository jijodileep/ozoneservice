using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Subscription;

namespace OzoneMobileService.Application.Features.UpgradeRequests.Commands;

public sealed record ApproveUpgradeRequestCommand(Guid RequestId, Guid ReviewerUserId)
    : ICommand<UpgradeRequestResponse?>;
