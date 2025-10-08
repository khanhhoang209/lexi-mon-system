using System.Security.Claims;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
  private readonly IOrderService _service;
  public OrdersController(IOrderService service)
  {
      _service = service;
  }

  [HttpPost]
  [Authorize]
  public async Task<IResult> CreateOrder([FromBody] OrderRequestDto request, CancellationToken cancellationToken)
  {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new Exception("User not found");

      var response = await _service.CreateOrder(request, userId, cancellationToken);
      if (!response.Succeeded)
        return TypedResults.BadRequest(response);
      
      return TypedResults.Created($"/api/orders/{((ResponseData<Guid>)response).Data}", response);
  }
    [HttpGet("users")]
     public async Task<IResult> GetAllUsersOrders(
         [FromQuery] GetOrderUserRequest request,
         CancellationToken cancellationToken)
     {
         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new Exception("User not found");
         var response = await _service.GetAllUsersOrdersByUserId(request, userId, cancellationToken);
         if (!response.Succeeded)
           return TypedResults.BadRequest(response);
         
         return TypedResults.Ok(response);
     }
}