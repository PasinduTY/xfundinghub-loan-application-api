using Microsoft.AspNetCore.Mvc;
using XFundingHub.Application.DTOs;
using XFundingHub.Application.Services;
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

            var response = new LoanApplicationResponse
            {
                ApplicationId = application.ApplicationId,
                CustomerId = application.CustomerId,
                Amount = application.Amount,
                Currency = application.Currency,
                TermMonths = application.TermMonths,
                Status = application.Status.ToString().ToUpperInvariant()
            };

            return CreatedAtAction(
                nameof(GetById),
                new { applicationId = application.ApplicationId },
                response);
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpGet("{applicationId}")]
    public async Task<IActionResult> GetById(string applicationId)
    {
        var application = await _service.GetByIdAsync(applicationId);

        if (application is null)
        {
            return NotFound();
        }

        var response = new LoanApplicationResponse
        {
            ApplicationId = application.ApplicationId,
            CustomerId = application.CustomerId,
            Amount = application.Amount,
            Currency = application.Currency,
            TermMonths = application.TermMonths,
            Status = application.Status.ToString().ToUpperInvariant()
        };

        return Ok(response);
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
            return BadRequest();
        }

        try
        {
            var application = await _service.ChangeStatusAsync(
                applicationId,
                newStatus);

            if (application is null)
            {
                return NotFound();
            }

            var response = new LoanApplicationResponse
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

            return Ok(response);
        }
        catch (InvalidStatusTransitionException)
        {
            return Conflict();
        }
    }
}