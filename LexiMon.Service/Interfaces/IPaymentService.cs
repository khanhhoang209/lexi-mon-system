using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using Net.payOS.Types;

namespace LexiMon.Service.Interfaces;

public interface IPaymentService
{
    Task<ServiceResponse> CreatePayment(PaymentRequest requestBody, CancellationToken cancellationToken = default);
    Task<ServiceResponse> HandleWebhook(WebhookType webhookType, CancellationToken cancellationToken = default);
}