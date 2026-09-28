using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XFundingHub.Domain.Entities;

namespace XFundingHub.Infrastructure.Data;

public class LoanApplicationConfiguration
    : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.HasKey(x => x.ApplicationId);

        builder.Property(x => x.ApplicationId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.TermMonths)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();
    }
}
