using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Free, Premium")]
    [HttpGet("profile")]
    public async Task<IResult> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _userService.GetUserByIdAsync(cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [Authorize(Roles = "Free, Premium")]
    [HttpPut("profile")]
    public async Task<IResult> UpdateProfileAsync([FromBody] UserRequestDto requestBody, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _userService.UpdateAsync(requestBody, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [Authorize(Roles = "Free, Premium")]
    [HttpPost("resource")]
    public async Task<IResult> UpdateResourceAsync([FromBody] UserResourseDto requestBody, CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _userService.UpdateResourceAsync(requestBody, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
}