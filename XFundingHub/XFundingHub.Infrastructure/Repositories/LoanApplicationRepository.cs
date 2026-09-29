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

    public async Task<int> GetNextApplicationNumberAsync()
    {
        var applicationIds = await _context.LoanApplications
            .Select(x => x.ApplicationId)
            .ToListAsync();

        var maxNumber = applicationIds
            .Select(id => int.TryParse(
                id.Replace("LA", ""),
                out var number)
                ? number
                : 0)
            .DefaultIfEmpty(1000)
            .Max();

        return maxNumber + 1;
    }

    public async Task UpdateAsync(LoanApplication application)
    {
        _context.LoanApplications.Update(application);

        await _context.SaveChangesAsync();
    }
}