using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardsController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardsController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }


    [HttpGet("revenue")]
    public async Task<IResult> GetRevenueAsync(
        [FromQuery] RevenueRequestDto requestBody,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetRevenueAsync(requestBody, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
}