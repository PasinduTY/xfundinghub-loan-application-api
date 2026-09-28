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

    public LoanApplication(
    string applicationId,
    string customerId,
    decimal amount,
    string currency,
    int termMonths)
    {
        Validate(customerId, amount, currency, termMonths);

        ApplicationId = applicationId;
        CustomerId = customerId;
        Amount = amount;
        Currency = currency;
        TermMonths = termMonths;
        Status = ApplicationStatus.Submitted;
    }

    public void ChangeStatus(ApplicationStatus newStatus)
    {
        if (!AllowedTransitions[Status].Contains(newStatus))
        {
            throw new InvalidStatusTransitionException(Status, newStatus);
        }

        Status = newStatus;
    }

    private static void Validate(
    string customerId,
    decimal amount,
    string currency,
    int termMonths)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));
        }

        if (amount < 10000m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be at least 10000.");
        }

        if (termMonths < 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(termMonths),
                "Term months must be at least 12.");
        }

        if (currency != "GBP" && currency != "gbp")
        {
            throw new ArgumentException(
                "Currency must be GBP.",
                nameof(currency));
        }
    }
}
