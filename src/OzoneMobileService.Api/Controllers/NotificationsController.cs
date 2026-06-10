using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OzoneMobileService.Application.DTOs.Notifications;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/notifications")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var items = await notificationService.GetForCurrentUserAsync(cancellationToken);
        return Ok(items);
    }

    [HttpPatch("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        await notificationService.MarkReadAsync(id, cancellationToken);
        return NoContent();
    }
}
