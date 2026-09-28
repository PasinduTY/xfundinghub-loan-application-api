using XFundingHub.Domain.Entities;
using XFundingHub.Domain.Enums;
using XFundingHub.Domain.Exceptions;

namespace XFundingHub.UnitTests;

public class LoanApplicationTests
{
    [Fact]
    public void ChangeStatus_FromSubmittedToUnderReview_ShouldUpdateStatus()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
           10000m,
            "GBP",
            12);

        // Act
        application.ChangeStatus(ApplicationStatus.UnderReview);

        // Assert
        Assert.Equal(ApplicationStatus.UnderReview, application.Status);
    }

    [Fact]
    public void ChangeStatus_FromSubmittedToApproved_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        // Act & Assert
        Assert.Throws<InvalidStatusTransitionException>(() =>
            application.ChangeStatus(ApplicationStatus.Approved));
    }

    [Fact]
    public void ChangeStatus_FromUnderReviewToApproved_ShouldUpdateStatus()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);

        // Act
        application.ChangeStatus(ApplicationStatus.Approved);

        // Assert
        Assert.Equal(ApplicationStatus.Approved, application.Status);
    }

    [Fact]
    public void ChangeStatus_FromUnderReviewToRejected_ShouldUpdateStatus()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);

        // Act
        application.ChangeStatus(ApplicationStatus.Rejected);

        // Assert
        Assert.Equal(ApplicationStatus.Rejected, application.Status);
    }

    [Fact]
    public void ChangeStatus_FromApprovedToDisbursed_ShouldUpdateStatus()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);
        application.ChangeStatus(ApplicationStatus.Approved);

        // Act
        application.ChangeStatus(ApplicationStatus.Disbursed);

        // Assert
        Assert.Equal(ApplicationStatus.Disbursed, application.Status);
    }

    [Fact]
    public void ChangeStatus_FromUnderReviewToDisbursed_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);

        // Act & Assert
        Assert.Throws<InvalidStatusTransitionException>(() =>
            application.ChangeStatus(ApplicationStatus.Disbursed));
    }

    [Theory]
    [InlineData(ApplicationStatus.Approved, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Approved, ApplicationStatus.UnderReview)]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Approved)]
    public void ChangeStatus_InvalidTransition_ShouldThrow(
    ApplicationStatus currentStatus,
    ApplicationStatus newStatus)
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        MoveApplicationToStatus(application, currentStatus);

        // Act & Assert
        Assert.Throws<InvalidStatusTransitionException>(() =>
            application.ChangeStatus(newStatus));
    }

    private static void MoveApplicationToStatus(
    LoanApplication application,
    ApplicationStatus targetStatus)
    {
        switch (targetStatus)
        {
            case ApplicationStatus.Submitted:
                break;

            case ApplicationStatus.UnderReview:
                application.ChangeStatus(ApplicationStatus.UnderReview);
                break;

            case ApplicationStatus.Approved:
                application.ChangeStatus(ApplicationStatus.UnderReview);
                application.ChangeStatus(ApplicationStatus.Approved);
                break;

            case ApplicationStatus.Rejected:
                application.ChangeStatus(ApplicationStatus.UnderReview);
                application.ChangeStatus(ApplicationStatus.Rejected);
                break;

            case ApplicationStatus.Disbursed:
                application.ChangeStatus(ApplicationStatus.UnderReview);
                application.ChangeStatus(ApplicationStatus.Approved);
                application.ChangeStatus(ApplicationStatus.Disbursed);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(targetStatus));
        }
    }

    [Fact]
    public void ChangeStatus_FromRejectedToUnderReview_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);
        application.ChangeStatus(ApplicationStatus.Rejected);

        // Act & Assert
        Assert.Throws<InvalidStatusTransitionException>(() =>
            application.ChangeStatus(ApplicationStatus.UnderReview));
    }

    [Fact]
    public void ChangeStatus_FromDisbursedToApproved_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);
        application.ChangeStatus(ApplicationStatus.Approved);
        application.ChangeStatus(ApplicationStatus.Disbursed);

        // Act & Assert
        Assert.Throws<InvalidStatusTransitionException>(() =>
            application.ChangeStatus(ApplicationStatus.Approved));
    }

    [Fact]
    public void ChangeStatus_InvalidTransition_ShouldIncludeCurrentAndRequestedStatus()
    {
        // Arrange
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        application.ChangeStatus(ApplicationStatus.UnderReview);

        // Act
        var exception = Assert.Throws<InvalidStatusTransitionException>(() =>
            application.ChangeStatus(ApplicationStatus.Disbursed));

        // Assert
        Assert.Equal(ApplicationStatus.UnderReview, exception.CurrentStatus);
        Assert.Equal(ApplicationStatus.Disbursed, exception.RequestedStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateLoanApplication_WithEmptyCustomerId_ShouldThrowArgumentException(
    string customerId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new LoanApplication(
                "LA1001",
                customerId,
                10000m,
                "GBP",
                12));
    }

    [Theory]
    [InlineData(9999)]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateLoanApplication_WithAmountBelowMinimum_ShouldThrowArgumentOutOfRangeException(
    decimal amount)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new LoanApplication(
                "LA1001",
                "C1001",
                amount,
                "GBP",
                12));
    }

    [Fact]
    public void CreateLoanApplication_WithMinimumAmount_ShouldCreateSuccessfully()
    {
        // Act
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        // Assert
        Assert.Equal(10000m, application.Amount);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateLoanApplication_WithTermMonthsBelowMinimum_ShouldThrowArgumentOutOfRangeException(
    int termMonths)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new LoanApplication(
                "LA1001",
                "C1001",
                10000m,
                "GBP",
                termMonths));
    }

    [Fact]
    public void CreateLoanApplication_WithMinimumTermMonths_ShouldCreateSuccessfully()
    {
        // Act
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            12);

        // Assert
        Assert.Equal(12, application.TermMonths);
    }

    [Fact]
    public void CreateLoanApplication_WithLargeTermMonths_ShouldCreateSuccessfully()
    {
        // Act
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            "GBP",
            120);

        // Assert
        Assert.Equal(120, application.TermMonths);
    }

    [Theory]
    [InlineData("GBP")]
    [InlineData("gbp")]
    public void CreateLoanApplication_WithSupportedCurrency_ShouldCreateSuccessfully(
    string currency)
    {
        // Act
        var application = new LoanApplication(
            "LA1001",
            "C1001",
            10000m,
            currency,
            12);

        // Assert
        Assert.Equal(currency, application.Currency);
    }

    [Theory]
    [InlineData("Gbp")]
    [InlineData("gBp")]
    [InlineData("GbP")]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("")]
    public void CreateLoanApplication_WithUnsupportedCurrency_ShouldThrowArgumentException(
    string currency)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new LoanApplication(
                "LA1001",
                "C1001",
                10000m,
                currency,
                12));
    }

    
}
