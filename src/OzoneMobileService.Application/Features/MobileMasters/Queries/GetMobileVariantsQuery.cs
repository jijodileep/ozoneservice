using OzoneMobileService.Application.Common.Abstractions;
using OzoneMobileService.Application.DTOs.MobileMasters;

namespace OzoneMobileService.Application.Features.MobileMasters.Queries;

public sealed record GetMobileVariantsQuery(Guid ModelId, bool ActiveOnly = false)
    : IQuery<IReadOnlyList<MobileVariantResponse>>;
