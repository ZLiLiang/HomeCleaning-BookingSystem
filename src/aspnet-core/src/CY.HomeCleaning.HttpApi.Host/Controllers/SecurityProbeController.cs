using CY.HomeCleaning.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CY.HomeCleaning.Controllers;

[ApiController]
[Route("api/security-probe")]
public class SecurityProbeController : HomeCleaningController
{
    [HttpGet("backoffice")]
    [Authorize(Policy = HomeCleaningAuthorizationPolicies.BackofficeOnly)]
    public IActionResult Backoffice()
    {
        return Ok(new { message = "backoffice policy passed" });
    }

    [HttpGet("customer")]
    [Authorize(Policy = HomeCleaningAuthorizationPolicies.CustomerOnly)]
    public IActionResult Customer()
    {
        return Ok(new { message = "customer policy passed" });
    }
}
