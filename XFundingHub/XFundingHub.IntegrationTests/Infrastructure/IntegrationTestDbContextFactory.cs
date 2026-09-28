using Microsoft.EntityFrameworkCore;
using XFundingHub.Infrastructure.Data;

namespace XFundingHub.IntegrationTests.Infrastructure;

public static class IntegrationTestDbContextFactory
{
    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;" +
        "Database=XFundingHubDb;" +
        "Trusted_Connection=True;" +
        "TrustServerCertificate=True;";

    public static XFundingHubDbContext Create()
    {
        var options = new DbContextOptionsBuilder<XFundingHubDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new XFundingHubDbContext(options);
    }
}