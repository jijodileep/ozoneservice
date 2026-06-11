using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Common;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Exceptions;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Services;

public class UpgradeRequestService(
    AppDbContext dbContext,
    ISubscriptionLimitService subscriptionLimitService,
    ITaxService taxService) : IUpgradeRequestService
{
    public async Task<UpgradeRequestResponse?> RequestUpgradeAsync(
        Guid tenantId,
        Guid planId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        var newPlan = await dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        if (tenant is null || newPlan is null)
        {
            return null;
        }

        if (newPlan.TierOrder <= tenant.SubscriptionPlan.TierOrder)
        {
            throw new PlanUpgradeException();
        }

        var hasPending = await dbContext.SubscriptionUpgradeRequests
            .AnyAsync(
                r => r.TenantId == tenantId && r.Status == UpgradeRequestStatuses.Pending,
                cancellationToken);

        if (hasPending)
        {
            throw new InvalidOperationException("An upgrade request is already pending approval.");
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(tenantId, planId, cancellationToken);

        var now = DateTime.UtcNow;
        var request = new SubscriptionUpgradeRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            RequestedPlanId = newPlan.Id,
            CurrentPlanId = tenant.SubscriptionPlanId,
            Status = UpgradeRequestStatuses.Pending,
            RequestedAt = now,
            CreatedAt = now
        };

        dbContext.SubscriptionUpgradeRequests.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        await CreateNotificationAsync(
            null,
            Roles.PlatformSuperAdmin,
            "Upgrade request",
            $"{tenant.Name} requested upgrade to {newPlan.Name} (₹{newPlan.Price}).",
            NotificationTypes.UpgradeRequested,
            cancellationToken);

        await CreateNotificationAsync(
            tenant.Id,
            Roles.TenantAdmin,
            "Upgrade requested",
            $"Your request to upgrade to {newPlan.Name} was submitted and is awaiting approval.",
            NotificationTypes.UpgradeRequested,
            cancellationToken);

        return await GetRequestByIdAsync(request.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<UpgradeRequestResponse>> GetTenantRequestsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var items = await LoadRequestsQuery()
            .Where(r => r.TenantId == tenantId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<PagedResult<UpgradeRequestResponse>> GetPendingRequestsPagedAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = LoadRequestsQuery().IgnoreQueryFilters();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UpgradeRequestResponse>(
            items.Select(Map).ToList(),
            total,
            page,
            pageSize);
    }

    public async Task<UpgradeRequestResponse?> ApproveRequestAsync(
        Guid requestId,
        Guid reviewerUserId,
        CancellationToken cancellationToken = default)
    {
        var request = await dbContext.SubscriptionUpgradeRequests
            .IgnoreQueryFilters()
            .Include(r => r.RequestedPlan)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        if (request is null || request.Status != UpgradeRequestStatuses.Pending)
        {
            return null;
        }

        var tenant = await dbContext.Tenants
            .Include(t => t.Branches)
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(
            tenant.Id,
            request.RequestedPlanId,
            cancellationToken);

        var taxConfigDto = await taxService.GetConfigurationAsync(cancellationToken);
        var taxEntity = await dbContext.TaxConfigurations
            .FirstAsync(t => t.Id == taxConfigDto.Id, cancellationToken);

        var subTotal = request.RequestedPlan.Price;
        var (cgst, sgst, totalTax) = TaxService.CalculateTax(subTotal, taxEntity);
        var branch = tenant.Branches.OrderBy(b => b.Name).FirstOrDefault();

        if (branch is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            BranchId = branch.Id,
            InvoiceNumber = await GenerateSubscriptionInvoiceNumberAsync(cancellationToken),
            CustomerName = tenant.Name,
            CustomerPhone = "—",
            SubTotal = subTotal,
            CgstAmount = cgst,
            SgstAmount = sgst,
            TaxAmount = totalTax,
            TotalAmount = subTotal + totalTax,
            InvoiceType = InvoiceTypes.Subscription,
            IssuedAt = now,
            Status = "Issued",
            CreatedAt = now
        };

        tenant.SubscriptionPlanId = request.RequestedPlanId;
        tenant.SubscriptionExpiresAt = now.AddMonths(request.RequestedPlan.BillingPeriodMonths);

        request.Status = UpgradeRequestStatuses.Approved;
        request.ReviewedAt = now;
        request.ReviewedByUserId = reviewerUserId;
        request.InvoiceId = invoice.Id;
        request.UpdatedAt = now;

        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync(cancellationToken);

        var message =
            $"{tenant.Name} upgraded to {request.RequestedPlan.Name}. Invoice {invoice.InvoiceNumber} generated (₹{invoice.TotalAmount:N2}).";

        await CreateNotificationAsync(
            tenant.Id,
            Roles.TenantAdmin,
            "Upgrade approved",
            message,
            NotificationTypes.UpgradeApproved,
            cancellationToken);

        return await GetRequestByIdAsync(request.Id, cancellationToken);
    }

    public async Task<UpgradeRequestResponse?> RejectRequestAsync(
        Guid requestId,
        Guid reviewerUserId,
        string? reason,
        CancellationToken cancellationToken = default)
    {
        var request = await dbContext.SubscriptionUpgradeRequests
            .IgnoreQueryFilters()
            .Include(r => r.RequestedPlan)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        if (request is null || request.Status != UpgradeRequestStatuses.Pending)
        {
            return null;
        }

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        request.Status = UpgradeRequestStatuses.Rejected;
        request.ReviewedAt = now;
        request.ReviewedByUserId = reviewerUserId;
        request.RejectionReason = reason?.Trim();
        request.UpdatedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);

        await CreateNotificationAsync(
            tenant.Id,
            Roles.TenantAdmin,
            "Upgrade rejected",
            $"Your request to upgrade to {request.RequestedPlan.Name} was rejected.{(string.IsNullOrWhiteSpace(reason) ? "" : $" Reason: {reason}")}",
            NotificationTypes.UpgradeRejected,
            cancellationToken);

        return await GetRequestByIdAsync(request.Id, cancellationToken);
    }

    private IQueryable<SubscriptionUpgradeRequest> LoadRequestsQuery() =>
        dbContext.SubscriptionUpgradeRequests
            .AsNoTracking()
            .Include(r => r.RequestedPlan)
            .Include(r => r.CurrentPlan)
            .Include(r => r.Tenant)
            .Include(r => r.Invoice);

    private async Task<UpgradeRequestResponse?> GetRequestByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var item = await LoadRequestsQuery()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        return item is null ? null : Map(item);
    }

    private static UpgradeRequestResponse Map(SubscriptionUpgradeRequest r) =>
        new(
            r.Id,
            r.TenantId,
            r.Tenant.Name,
            r.CurrentPlan.Name,
            r.RequestedPlan.Name,
            r.RequestedPlan.Price,
            r.Status,
            r.RequestedAt,
            r.ReviewedAt,
            r.RejectionReason,
            r.InvoiceId,
            r.Invoice?.InvoiceNumber);

    private async Task<string> GenerateSubscriptionInvoiceNumberAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"SUB-{year}-";
        var count = await dbContext.Invoices
            .IgnoreQueryFilters()
            .CountAsync(i => i.InvoiceNumber.StartsWith(prefix), cancellationToken);

        return $"{prefix}{(count + 1):D4}";
    }

    private async Task CreateNotificationAsync(
        Guid? tenantId,
        string roleTarget,
        string title,
        string message,
        string notificationType,
        CancellationToken cancellationToken)
    {
        dbContext.AppNotifications.Add(new AppNotification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            RoleTarget = roleTarget,
            Title = title,
            Message = message,
            NotificationType = notificationType,
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
