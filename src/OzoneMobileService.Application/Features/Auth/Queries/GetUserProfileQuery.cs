using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.Auth;

namespace OzoneMobileService.Application.Features.Auth.Queries;

public sealed record GetUserProfileQuery(Guid UserId) : IQuery<UserProfileResponse?>;
