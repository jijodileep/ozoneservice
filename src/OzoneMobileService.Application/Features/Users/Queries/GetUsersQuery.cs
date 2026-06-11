using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Users;

namespace OzoneMobileService.Application.Features.Users.Queries;

public sealed record GetUsersQuery(Guid TenantId, Guid UserId) : IQuery<IReadOnlyList<UserResponse>>;
