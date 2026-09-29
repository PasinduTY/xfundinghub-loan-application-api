using XFundingHub.Application.Repositories;
using XFundingHub.Domain.Entities;
using XFundingHub.Domain.Enums;

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
        var applicationNumber =
            await _repository.GetNextApplicationNumberAsync();

        var applicationId =
            _idGenerator.Generate(applicationNumber);

        var application = new LoanApplication(
            applicationId,
            customerId,
            amount,
            currency,
            termMonths);

        await _repository.AddAsync(application);

        return application;
    }

    public async Task<LoanApplication?> GetByIdAsync(string applicationId)
    {
        return await _repository.GetByIdAsync(applicationId);
    }

    public async Task<LoanApplication?> ChangeStatusAsync(
    string applicationId,
    ApplicationStatus newStatus)
    {
        var application = await _repository.GetByIdAsync(applicationId);

        if (application is null)
        {
            return null;
        }

        application.ChangeStatus(newStatus);

        await _repository.UpdateAsync(application);

        return application;
    }
}