using XFundingHub.Domain.Entities;
using XFundingHub.Domain.Services;

namespace XFundingHub.Application.Services;

public class LoanApplicationService
{
    private readonly ApplicationIdGenerator _idGenerator;

    public LoanApplicationService(ApplicationIdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }

    public LoanApplication Create(
        string customerId,
        decimal amount,
        string currency,
        int termMonths)
    {
        var applicationId = _idGenerator.Generate();

        return new LoanApplication(
            applicationId,
            customerId,
            amount,
            currency,
            termMonths);
    }
}
