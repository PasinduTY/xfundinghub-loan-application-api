using Microsoft.EntityFrameworkCore;
using XFundingHub.Domain.Entities;

namespace XFundingHub.Infrastructure.Data;

public class XFundingHubDbContext : DbContext
{
    public XFundingHubDbContext(
        DbContextOptions<XFundingHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(
            new LoanApplicationConfiguration());
    }
}