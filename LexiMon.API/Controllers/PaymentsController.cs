using System.Text;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("{orderId:Guid}")]
    [Authorize(Roles = "Free")]
    public async Task<IResult> CreatePayment([FromRoute] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var request = new PaymentRequest
        {
            OrderId = orderId
        };

        var serviceResponse = await _paymentService.CreatePayment(request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpGet("return")]
    [Authorize(Roles = "Free")]
    public async Task<IResult> PaymentReturn([FromQuery] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _paymentService.PaymentReturn(orderId, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpGet("cancel")]
    [Authorize(Roles = "Free")]
    public async Task<IResult> PaymentCancel([FromQuery] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _paymentService.PaymentCancel(orderId, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

}