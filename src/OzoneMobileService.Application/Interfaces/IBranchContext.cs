namespace OzoneMobileService.Application.Interfaces;

public interface IBranchContext
{
    /// <summary>Selected branch from X-Branch-Id header or the user's default branch.</summary>
    Guid? BranchId { get; }

    bool HasBranch => BranchId.HasValue;
}
