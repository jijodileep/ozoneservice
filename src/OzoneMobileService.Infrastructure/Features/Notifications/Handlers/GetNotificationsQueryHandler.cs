using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Notifications;
using OzoneMobileService.Application.Features.Notifications.Queries;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.Notifications.Handlers;

internal sealed class GetNotificationsQueryHandler(
    AppDbContext dbContext,
    ITenantContext tenantContext,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationResponse>>
{
    public async Task<IReadOnlyList<NotificationResponse>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await GetCurrentUserRolesAsync();
        if (roles.Count == 0)
        {
            return [];
        }

        var query = dbContext.AppNotifications.AsNoTracking().Where(n => !n.IsRead);

        if (tenantContext.IsPlatformAdmin)
        {
            query = query.Where(n => n.RoleTarget == Roles.PlatformSuperAdmin);
        }
        else if (tenantContext.HasTenant)
        {
            query = query.Where(n =>
                n.RoleTarget == Roles.TenantAdmin && n.TenantId == tenantContext.TenantId);
        }
        else
        {
            return [];
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new NotificationResponse(
                n.Id,
                n.TenantId,
                n.Title,
                n.Message,
                n.NotificationType,
                n.IsRead,
                n.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<string>> GetCurrentUserRolesAsync()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return [];
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return [];
        }

        return (await userManager.GetRolesAsync(user)).ToList();
    }
}
