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
        var serviceResponse =
            await _dashboardService.GetRevenueAsync(requestBody, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpGet("total-users")]
    public async Task<IResult> GetTotalUsersAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetTotalUsersAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("total-courses")]
    public async Task<IResult> GetTotalCoursesAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetTotalCoursesAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("total-coins")]
    public async Task<IResult> GetTotalCoinsAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetTotalCoinsAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("total-items")]
    public async Task<IResult> GetTotalItemsAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetTotalItemsAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("subscription")]
    public async Task<IResult> GetSubscriptionAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetSubscriptionAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("popular-languages")]
    public async Task<IResult> GetPopularLanguagesAsync(
        [FromQuery] PopularLanguageRequestDto requestBody,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse =
            await _dashboardService.GetPopularLanguagesAsync(requestBody, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("weekly-revenue")]
    public async Task<IResult> GetWeeklyRevenueAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetWeeklyRevenueAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("monthly-revenue")]
    public async Task<IResult> GetMonthlyRevenueAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetMonthlyRevenueAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpGet("yearly-revenue")]
    public async Task<IResult> GetYearlyRevenueAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _dashboardService.GetYearlyRevenueAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

}