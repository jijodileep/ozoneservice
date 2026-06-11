using MediatR;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.DTOs.Branches;
using OzoneMobileService.Application.Features.Branches.Commands;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Entities;
using OzoneMobileService.Infrastructure.Persistence;

namespace OzoneMobileService.Infrastructure.Features.Branches.Handlers;

internal sealed class CreateBranchCommandHandler(
    AppDbContext dbContext,
    BranchAccessService branchAccess,
    ISubscriptionLimitService subscriptionLimitService)
    : IRequestHandler<CreateBranchCommand, BranchResponse?>
{
    public async Task<BranchResponse?> Handle(
        CreateBranchCommand request,
        CancellationToken cancellationToken)
    {
        if (!await branchAccess.IsTenantAdminAsync(request.UserId, cancellationToken))
        {
            return null;
        }

        await subscriptionLimitService.ValidateCanAddBranchAsync(request.TenantId, cancellationToken);

        var code = request.Code.Trim().ToUpperInvariant();
        if (await dbContext.Branches.AnyAsync(
                b => b.TenantId == request.TenantId && b.Code == code,
                cancellationToken))
        {
            return null;
        }

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Code = code,
            Name = request.Name.Trim(),
            Address = BranchMapper.NormalizeOptional(request.Address),
            Phone = BranchMapper.NormalizePhone(request.Phone),
            GstNumber = BranchMapper.NormalizeGstNumber(request.GstNumber),
            IsActive = true
        };

        dbContext.Branches.Add(branch);
        await dbContext.SaveChangesAsync(cancellationToken);
        return BranchMapper.Map(branch);
    }
}
