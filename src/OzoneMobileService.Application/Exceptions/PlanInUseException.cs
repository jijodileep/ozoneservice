namespace OzoneMobileService.Application.Exceptions;

public class PlanInUseException()
    : Exception("Plan is assigned to one or more shops and cannot be deleted.");
