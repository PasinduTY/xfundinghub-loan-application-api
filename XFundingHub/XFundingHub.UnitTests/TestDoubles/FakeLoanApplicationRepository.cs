using XFundingHub.Application.Repositories;
using XFundingHub.Domain.Entities;

namespace XFundingHub.UnitTests.TestDoubles;

public class FakeLoanApplicationRepository : ILoanApplicationRepository
{
    private readonly List<LoanApplication> _applications = [];

    public void Add(LoanApplication application)
    {
        _applications.Add(application);
    }

    public LoanApplication? GetById(string applicationId)
    {
        return _applications.FirstOrDefault(
            application => application.ApplicationId == applicationId);
    }
}