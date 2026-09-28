using XFundingHub.Application.Repositories;
using XFundingHub.Domain.Entities;

namespace XFundingHub.Application.Services;

public class LoanApplicationService
{
    private readonly ApplicationIdGenerator _idGenerator;
    private readonly ILoanApplicationRepository _repository;

    public LoanApplicationService(
        ApplicationIdGenerator idGenerator,
        ILoanApplicationRepository repository)
    {
        _idGenerator = idGenerator;
        _repository = repository;
    }

    public LoanApplication Create(
    string customerId,
    decimal amount,
    string currency,
    int termMonths)
    {
        var applicationId = _idGenerator.Generate();

        var application = new LoanApplication(
            applicationId,
            customerId,
            amount,
            currency,
            termMonths);

        _repository.Add(application);

        return application;
    }
}
