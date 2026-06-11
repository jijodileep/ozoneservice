using OzoneMobileService.Domain.Common;

namespace OzoneMobileService.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Normalized 10-digit mobile number (India).</summary>
    public string MobileNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public ICollection<CustomerDevice> Devices { get; set; } = [];
}
