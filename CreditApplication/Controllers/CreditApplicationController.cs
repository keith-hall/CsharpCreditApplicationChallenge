using Microsoft.AspNetCore.Mvc;
using CreditApplication.Models;
using CreditApplication.Services;
using System.Net.Mime;

namespace CreditApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class CreditApplicationController : ControllerBase
{
    private readonly ICreditApplicationService _service;

    public CreditApplicationController(ICreditApplicationService creditApplicationService)
    {
        _service = creditApplicationService;
    }

    [HttpPost(Name = "ComputeCreditApplication")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CreditApplicationResponse> ComputeCreditApplication(CreditApplicationRequest application)
    {
        if (_service.GetValidationErrors(application).Any())
        {
            return BadRequest();
        }

        return _service.MakeDecision(application);
    }
}
