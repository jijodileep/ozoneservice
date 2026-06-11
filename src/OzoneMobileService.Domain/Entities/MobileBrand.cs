namespace OzoneMobileService.Domain.Entities;

public class MobileBrand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<MobileModel> Models { get; set; } = [];
}
