using Microsoft.AspNetCore.Mvc;
using XFundingHub.Application.DTOs;
using XFundingHub.Application.Services;
using XFundingHub.Domain.Entities;
using XFundingHub.Domain.Enums;
using XFundingHub.Domain.Exceptions;

namespace XFundingHub.API.Controllers;

[ApiController]
[Route("api/v1/loan-applications")]
public class LoanApplicationsController : ControllerBase
{
    private readonly LoanApplicationService _service;

    public LoanApplicationsController(LoanApplicationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLoanApplicationRequest request)
    {
        try
        {
            var application = await _service.CreateAsync(
                request.CustomerId,
                request.Amount,
                request.Currency,
                request.TermMonths);

            var response = ToResponse(application);

            return CreatedAtAction(
                nameof(GetById),
                new { applicationId = application.ApplicationId },
                response);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{applicationId}")]
    public async Task<IActionResult> GetById(string applicationId)
    {
        var application = await _service.GetByIdAsync(applicationId);

        if (application is null)
{
            return NotFound(new
            {
                message = $"Loan application '{applicationId}' was not found."
            });
        }

        return Ok(ToResponse(application));
    }

    [HttpPatch("{applicationId}/status")]
    public async Task<IActionResult> ChangeStatus(
        string applicationId,
        [FromBody] ChangeLoanApplicationStatusRequest request)
    {
        if (!Enum.TryParse<ApplicationStatus>(
        request.Status.Replace("_", ""),
        true,
        out var newStatus))
        {
            return BadRequest(new
            {
                message = $"Unsupported status '{request.Status}'."
            });
        }

        try
        {
            var application = await _service.ChangeStatusAsync(
                applicationId,
                newStatus);

            if (application is null)
            {
                return NotFound(new
                {
                    message = $"Loan application '{applicationId}' was not found."
                });
            }

            return Ok(ToResponse(application));
        }
        catch (InvalidStatusTransitionException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    private static LoanApplicationResponse ToResponse(
        LoanApplication application)
    {
        return new LoanApplicationResponse
        {
            ApplicationId = application.ApplicationId,
            CustomerId = application.CustomerId,
            Amount = application.Amount,
            Currency = application.Currency,
            TermMonths = application.TermMonths,
            Status = application.Status switch
            {
                ApplicationStatus.Submitted => "SUBMITTED",
                ApplicationStatus.UnderReview => "UNDER_REVIEW",
                ApplicationStatus.Approved => "APPROVED",
                ApplicationStatus.Rejected => "REJECTED",
                ApplicationStatus.Disbursed => "DISBURSED",
                _ => application.Status.ToString().ToUpperInvariant()
            }
        };
    }
}