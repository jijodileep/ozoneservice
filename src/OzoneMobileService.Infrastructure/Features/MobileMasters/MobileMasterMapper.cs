using OzoneMobileService.Application.DTOs.MobileMasters;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Features.MobileMasters;

internal static class MobileMasterMapper
{
    internal static MobileBrandResponse MapBrand(MobileBrand brand) =>
        new(brand.Id, brand.Name, brand.IsActive, brand.TenantId);

    internal static MobileModelResponse MapModel(MobileModel model) =>
        new(model.Id, model.BrandId, model.Name, model.IsActive, model.TenantId);

    internal static MobileVariantResponse MapVariant(MobileVariant variant) =>
        new(variant.Id, variant.ModelId, variant.Name, variant.IsActive, variant.TenantId);

    internal static string NormalizeName(string name) => name.Trim();
}
