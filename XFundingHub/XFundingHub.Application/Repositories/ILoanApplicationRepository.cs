using XFundingHub.Domain.Entities;

namespace XFundingHub.Application.Repositories;

public interface ILoanApplicationRepository
{
    void Add(LoanApplication application);

    LoanApplication? GetById(string applicationId);
}