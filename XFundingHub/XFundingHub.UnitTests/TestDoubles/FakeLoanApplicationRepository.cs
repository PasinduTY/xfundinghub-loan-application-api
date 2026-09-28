using XFundingHub.Application.Repositories;
using XFundingHub.Domain.Entities;

namespace XFundingHub.UnitTests.TestDoubles;

public class FakeLoanApplicationRepository : ILoanApplicationRepository
{
    private readonly List<LoanApplication> _applications = [];

    public Task AddAsync(LoanApplication application)
    {
        _applications.Add(application);

        return Task.CompletedTask;
    }

    public Task<LoanApplication?> GetByIdAsync(string applicationId)
    {
        var application = _applications.FirstOrDefault(
            application => application.ApplicationId == applicationId);

        return Task.FromResult(application);
    }
}