using Microsoft.AspNetCore.Http;
using OzoneMobileService.Application.Interfaces;

namespace OzoneMobileService.Infrastructure.Authorization;

public class BranchContext(IHttpContextAccessor httpContextAccessor) : IBranchContext
{
    public const string BranchIdItemKey = "BranchId";

    public Guid? BranchId =>
        httpContextAccessor.HttpContext?.Items[BranchIdItemKey] as Guid?;
}
