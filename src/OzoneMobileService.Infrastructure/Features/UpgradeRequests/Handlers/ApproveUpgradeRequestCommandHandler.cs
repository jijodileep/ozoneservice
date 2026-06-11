using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Subscription;
using OzoneMobileService.Application.Features.UpgradeRequests.Commands;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Features.Platform;
using OzoneMobileService.Infrastructure.Persistence;
using OzoneMobileService.Shared;

namespace OzoneMobileService.Infrastructure.Features.UpgradeRequests.Handlers;

internal sealed class ApproveUpgradeRequestCommandHandler(
    AppDbContext dbContext,
    ISubscriptionLimitService subscriptionLimitService,
    NotificationWriter notificationWriter)
    : IRequestHandler<ApproveUpgradeRequestCommand, UpgradeRequestResponse?>
{
    public async Task<UpgradeRequestResponse?> Handle(
        ApproveUpgradeRequestCommand command,
        CancellationToken cancellationToken)
    {
        var upgradeRequest = await dbContext.SubscriptionUpgradeRequests
            .IgnoreQueryFilters()
            .Include(r => r.RequestedPlan)
            .FirstOrDefaultAsync(r => r.Id == command.RequestId, cancellationToken);

        if (upgradeRequest is null || upgradeRequest.Status != UpgradeRequestStatuses.Pending)
        {
            return null;
        }

        var tenant = await dbContext.Tenants
            .Include(t => t.Branches)
            .FirstOrDefaultAsync(t => t.Id == upgradeRequest.TenantId, cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        await subscriptionLimitService.ValidatePlanAssignmentAsync(
            tenant.Id,
            upgradeRequest.RequestedPlanId,
            cancellationToken);

        var taxEntity = await TaxConfigurationAccessor.GetOrCreateActiveAsync(dbContext, cancellationToken);

        var subTotal = upgradeRequest.RequestedPlan.Price;
        var (cgst, sgst, totalTax) = TaxCalculator.CalculateTax(subTotal, taxEntity);
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

        tenant.SubscriptionPlanId = upgradeRequest.RequestedPlanId;
        tenant.SubscriptionExpiresAt = now.AddMonths(upgradeRequest.RequestedPlan.BillingPeriodMonths);

        upgradeRequest.Status = UpgradeRequestStatuses.Approved;
        upgradeRequest.ReviewedAt = now;
        upgradeRequest.ReviewedByUserId = command.ReviewerUserId;
        upgradeRequest.InvoiceId = invoice.Id;
        upgradeRequest.UpdatedAt = now;

        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync(cancellationToken);

        var message =
            $"{tenant.Name} upgraded to {upgradeRequest.RequestedPlan.Name}. Invoice {invoice.InvoiceNumber} generated (₹{invoice.TotalAmount:N2}).";

        await notificationWriter.CreateAsync(
            tenant.Id,
            Roles.TenantAdmin,
            "Upgrade approved",
            message,
            NotificationTypes.UpgradeApproved,
            cancellationToken);

        return await GetRequestByIdAsync(upgradeRequest.Id, cancellationToken);
    }

    private async Task<UpgradeRequestResponse?> GetRequestByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var item = await GetTenantUpgradeRequestsQueryHandler.LoadRequestsQuery(dbContext)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        return item is null ? null : UpgradeRequestMapper.Map(item);
    }

    private async Task<string> GenerateSubscriptionInvoiceNumberAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"SUB-{year}-";
        var count = await dbContext.Invoices
            .IgnoreQueryFilters()
            .CountAsync(i => i.InvoiceNumber.StartsWith(prefix), cancellationToken);

        return $"{prefix}{(count + 1):D4}";
    }
}
