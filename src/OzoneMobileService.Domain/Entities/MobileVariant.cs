namespace OzoneMobileService.Domain.Entities;

public class MobileVariant
{
    public Guid Id { get; set; }

    public Guid ModelId { get; set; }

    public MobileModel Model { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
