using XFundingHub.Domain.Enums;

namespace XFundingHub.Domain.Exceptions;

public class InvalidStatusTransitionException : Exception
{
    public ApplicationStatus CurrentStatus { get; }

    public ApplicationStatus RequestedStatus { get; }

    public InvalidStatusTransitionException(
        ApplicationStatus currentStatus,
        ApplicationStatus requestedStatus)
        : base($"Cannot transition application from {currentStatus} to {requestedStatus}.")
    {
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }
}
