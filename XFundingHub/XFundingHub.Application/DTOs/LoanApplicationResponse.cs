namespace XFundingHub.Application.DTOs;

public class LoanApplicationResponse
{
    public string ApplicationId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public int TermMonths { get; set; }

    public string Status { get; set; } = string.Empty;
}