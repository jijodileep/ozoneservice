using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Features.Notifications.Commands;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Notifications.Handlers;

internal sealed class MarkNotificationReadCommandHandler(AppDbContext dbContext)
    : IRequestHandler<MarkNotificationReadCommand>
{
    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await dbContext.AppNotifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (notification is null)
        {
            return;
        }

        notification.IsRead = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
