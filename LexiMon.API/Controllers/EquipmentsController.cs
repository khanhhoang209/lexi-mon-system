using System.Security.Claims;
using LexiMon.Service.Implements;
using LexiMon.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/equipments")]
public class EquipmentsController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentsController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEquipmentsAsync(
        [FromQuery] GetEquipmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");

        var result = await _equipmentService.GetEquipmentsAsync(userId, request, cancellationToken);
        return Ok(result);
    }
}