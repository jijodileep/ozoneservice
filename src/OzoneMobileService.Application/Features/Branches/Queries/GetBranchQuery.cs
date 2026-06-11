using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Branches;

namespace OzoneMobileService.Application.Features.Branches.Queries;

public sealed record GetBranchQuery(Guid BranchId, Guid UserId) : IQuery<BranchResponse?>;
