using OzoneMobileService.Domain.Common;

namespace OzoneMobileService.Domain.Entities;

public class MobileModel : BaseEntity
{
    public Guid BrandId { get; set; }

    public MobileBrand Brand { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<MobileVariant> Variants { get; set; } = [];
}
