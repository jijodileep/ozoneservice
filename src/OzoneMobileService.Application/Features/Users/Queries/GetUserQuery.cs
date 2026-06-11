using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Users;

namespace OzoneMobileService.Application.Features.Users.Queries;

public sealed record GetUserQuery(Guid Id, Guid TenantId, Guid UserId) : IQuery<UserResponse?>;
