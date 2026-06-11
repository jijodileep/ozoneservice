using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Notifications;
using OzoneMobileService.Application.Features.Notifications.Commands;
using OzoneMobileService.Application.Features.Notifications.Queries;

namespace OzoneMobileService.Api.Controllers;

[Route("api/notifications")]
public class NotificationsController(IMediator mediator) : AuthorizedApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var items = await mediator.Send(new GetNotificationsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpPatch("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkNotificationReadCommand(id), cancellationToken);
        return NoContent();
    }
}
