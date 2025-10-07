using System.Text;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
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
    public async Task<IResult> PaymentReturn([FromQuery] long orderCode,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _paymentService.PaymentReturn(orderCode, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpGet("cancel")]
    public async Task<IResult> PaymentCancel([FromQuery] long orderCode,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _paymentService.PaymentCancel(orderCode, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

}