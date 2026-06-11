namespace OzoneMobileService.Application.Exceptions;

public sealed class CustomerDuplicateException(string mobileNumber)
    : Exception($"A customer with mobile number {mobileNumber} already exists.")
{
    public string MobileNumber { get; } = mobileNumber;
}
