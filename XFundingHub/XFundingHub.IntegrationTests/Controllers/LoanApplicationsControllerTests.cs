using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using XFundingHub.Application.DTOs;

namespace XFundingHub.IntegrationTests.Controllers;

public class LoanApplicationsControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoanApplicationsControllerTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateLoanApplication_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            customerId = "C2001",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(result);
        Assert.NotNull(result.ApplicationId);
        Assert.Equal("C2001", result.CustomerId);
        Assert.Equal(10000m, result.Amount);
        Assert.Equal("GBP", result.Currency);
        Assert.Equal(12, result.TermMonths);
        Assert.Equal("SUBMITTED", result.Status);
    }

    [Fact]
    public async Task CreateLoanApplication_WithAmountBelowMinimum_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            customerId = "C2002",
            amount = 9999m,
            currency = "GBP",
            termMonths = 12
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Amount must be at least 10000", body);
    }

    [Fact]
    public async Task CreateLoanApplication_WithTermBelowMinimum_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            customerId = "C2004",
            amount = 10000m,
            currency = "GBP",
            termMonths = 11
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Term months must be at least", body);
    }

    [Fact]
    public async Task CreateLoanApplication_WithUnsupportedCurrency_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            customerId = "C2005",
            amount = 10000m,
            currency = "USD",
            termMonths = 12
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Currency must be GBP", body);
    }

    [Fact]
    public async Task CreateLoanApplication_WithEmptyCustomerId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            customerId = "",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Customer ID cannot be empty", body);
    }

    [Fact]
    public async Task GetLoanApplication_WithExistingApplication_ShouldReturnOk()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C3001",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLoanApplication_WithNonExistingApplication_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/v1/loan-applications/LA999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("LA999999", body);
        Assert.Contains("not found", body);
    }

    [Fact]
    public async Task ChangeStatus_FromSubmittedToUnderReview_ShouldReturnOk()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4001",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        var statusRequest = new
        {
            status = "UNDER_REVIEW"
        };

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            statusRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result =
            await response.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(result);
        Assert.Equal(
            "UNDER_REVIEW",
            result.Status);
    }

    [Fact]
    public async Task ChangeStatus_FromUnderReviewToApproved_ShouldReturnOk()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4002",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "UNDER_REVIEW" });

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "APPROVED" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result =
            await response.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(result);
        Assert.Equal("APPROVED", result.Status);
    }

    [Fact]
    public async Task ChangeStatus_FromUnderReviewToRejected_ShouldReturnOk()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4003",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "UNDER_REVIEW" });

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "REJECTED" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result =
            await response.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(result);
        Assert.Equal("REJECTED", result.Status);
    }

    [Fact]
    public async Task ChangeStatus_FromApprovedToDisbursed_ShouldReturnOk()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4004",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "UNDER_REVIEW" });

        await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "APPROVED" });

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "DISBURSED" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result =
            await response.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(result);
        Assert.Equal("DISBURSED", result.Status);
    }

    [Fact]
    public async Task ChangeStatus_FromSubmittedToApproved_ShouldReturnConflict()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4005",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "APPROVED" });

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Submitted", body);
        Assert.Contains("Approved", body);
    }

    [Fact]
    public async Task ChangeStatus_FromUnderReviewToDisbursed_ShouldReturnConflict()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4006",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "UNDER_REVIEW" });

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "DISBURSED" });

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("UnderReview", body);
        Assert.Contains("Disbursed", body);
    }

    [Fact]
    public async Task ChangeStatus_WithNonExistingApplication_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PatchAsJsonAsync(
            "/api/v1/loan-applications/LA999999/status",
            new { status = "UNDER_REVIEW" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("LA999999", body);
        Assert.Contains("not found", body);
    }

    [Fact]
    public async Task ChangeStatus_WithUnsupportedStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4007",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "INVALID_STATUS" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Unsupported status", body);
        Assert.Contains("INVALID_STATUS", body);
    }

    [Fact]
    public async Task ChangeStatus_WithInvalidStatusFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4008",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "underreview" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_WithUndefinedNumericStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            customerId = "C4009",
            amount = 10000m,
            currency = "GBP",
            termMonths = 12
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/loan-applications",
            createRequest);

        var createdApplication =
            await createResponse.Content.ReadFromJsonAsync<LoanApplicationResponse>();

        Assert.NotNull(createdApplication);

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/loan-applications/{createdApplication.ApplicationId}/status",
            new { status = "99" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}