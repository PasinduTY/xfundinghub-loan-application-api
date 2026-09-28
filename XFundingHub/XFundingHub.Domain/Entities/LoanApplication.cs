using XFundingHub.Domain.Enums;
using XFundingHub.Domain.Exceptions;

namespace XFundingHub.Domain.Entities;

public class LoanApplication
{
    public string ApplicationId { get; private set; }
    public string CustomerId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public int TermMonths { get; private set; }
    public ApplicationStatus Status { get; private set; }

    public LoanApplication(
        string applicationId,
        string customerId,
        decimal amount,
        string currency,
        int termMonths)
    {
        ApplicationId = applicationId;
        CustomerId = customerId;
        Amount = amount;
        Currency = currency;
        TermMonths = termMonths;
        Status = ApplicationStatus.Submitted;
    }

    private static readonly IReadOnlyDictionary<ApplicationStatus, ApplicationStatus[]> AllowedTransitions =
    new Dictionary<ApplicationStatus, ApplicationStatus[]>
    {
        [ApplicationStatus.Submitted] =
            [ApplicationStatus.UnderReview],

        [ApplicationStatus.UnderReview] =
            [ApplicationStatus.Approved, ApplicationStatus.Rejected],

        [ApplicationStatus.Approved] =
            [ApplicationStatus.Disbursed],

        [ApplicationStatus.Rejected] =
            [],

        [ApplicationStatus.Disbursed] =
            []
    };

    public void ChangeStatus(ApplicationStatus newStatus)
    {
        if (!AllowedTransitions[Status].Contains(newStatus))
        {
            throw new InvalidStatusTransitionException(Status, newStatus);
        }

        Status = newStatus;
    }
}
