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

    public async Task<LoanApplication> CreateAsync(
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

        await _repository.AddAsync(application);

        return application;
    }
}