using System.Text;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IResult> CreatePayment([FromRoute] Guid orderId, CancellationToken cancellationToken = default)
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

    [HttpPost("webhook")]
    public async Task<IResult> HandleWebhook(CancellationToken cancellationToken = default)
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;

        var result = await _paymentService.HandleWebhook(rawBody, cancellationToken);
        if (result.Succeeded) return TypedResults.Ok();
        return TypedResults.BadRequest(result);
    }
}