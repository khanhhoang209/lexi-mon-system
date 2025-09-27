using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/user-decks")]
public class UserDecksController : ControllerBase
{
    private readonly IUserDeckService _userDeckService;
    public UserDecksController(IUserDeckService userDeckService)
    {
        _userDeckService = userDeckService;
    }
    
    [HttpGet("")]
    [Authorize] 
    public async Task<IActionResult> GetUserDecksByUserIdAsync(
        [FromQuery] GetUserDeckRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? throw new Exception("User not found");
        var serviceResponse = await _userDeckService.GetUserDecksByUserIdAsync(userId, request, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return BadRequest(serviceResponse);
        }

        return Ok(serviceResponse);
    }

    [HttpGet("{id:guid}")]
    [Authorize] 
    public async Task<IActionResult> GetUserDeckByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _userDeckService.GetUserDeckByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return NotFound(serviceResponse);
        }

        return Ok(serviceResponse);
    }
    
}