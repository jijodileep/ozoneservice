using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Branches;

namespace OzoneMobileService.Application.Features.Branches.Queries;

public sealed record GetBranchesQuery(Guid UserId) : IQuery<IReadOnlyList<BranchResponse>>;
