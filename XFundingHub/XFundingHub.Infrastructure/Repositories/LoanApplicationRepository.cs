using Microsoft.EntityFrameworkCore;
using XFundingHub.Application.Repositories;
using XFundingHub.Domain.Entities;
using XFundingHub.Infrastructure.Data;

namespace XFundingHub.Infrastructure.Repositories;

public class LoanApplicationRepository : ILoanApplicationRepository
{
    private readonly XFundingHubDbContext _context;

    public LoanApplicationRepository(XFundingHubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LoanApplication application)
    {
        _context.LoanApplications.Add(application);

        await _context.SaveChangesAsync();
    }

    public async Task<LoanApplication?> GetByIdAsync(string applicationId)
    {
        return await _context.LoanApplications
            .FirstOrDefaultAsync(x => x.ApplicationId == applicationId);
    }
}