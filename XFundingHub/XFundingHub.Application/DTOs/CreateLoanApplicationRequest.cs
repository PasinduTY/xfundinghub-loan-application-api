namespace XFundingHub.Application.DTOs;

public class CreateLoanApplicationRequest
{
    public string CustomerId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public int TermMonths { get; set; }
}
