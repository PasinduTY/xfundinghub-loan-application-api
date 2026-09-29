using XFundingHub.Domain.Entities;

namespace XFundingHub.Application.Repositories;

public interface ILoanApplicationRepository
{
    Task AddAsync(LoanApplication application);

    Task<LoanApplication?> GetByIdAsync(string applicationId);

    Task<int> GetNextApplicationNumberAsync();

    Task UpdateAsync(LoanApplication application);
}