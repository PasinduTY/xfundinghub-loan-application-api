using XFundingHub.Application.Repositories;
using XFundingHub.Domain.Entities;

namespace XFundingHub.UnitTests.TestDoubles;

public class FakeLoanApplicationRepository : ILoanApplicationRepository
{
    private readonly Dictionary<string, LoanApplication> _applications = new();

    public Task AddAsync(LoanApplication application)
    {
        _applications[application.ApplicationId] = application;

        return Task.CompletedTask;
    }

    public Task<LoanApplication?> GetByIdAsync(string applicationId)
    {
        _applications.TryGetValue(applicationId, out var application);

        return Task.FromResult(application);
    }

    public Task<int> GetNextApplicationNumberAsync()
    {
        if (_applications.Count == 0)
        {
            return Task.FromResult(1001);
        }

        var maxNumber = _applications.Keys
            .Select(id => int.Parse(id.Replace("LA", "")))
            .Max();

        return Task.FromResult(maxNumber + 1);
    }

    public Task UpdateAsync(LoanApplication application)
    {
        _applications[application.ApplicationId] = application;

        return Task.CompletedTask;
    }
}