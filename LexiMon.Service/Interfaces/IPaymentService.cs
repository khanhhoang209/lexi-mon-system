using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;

namespace LexiMon.Service.Interfaces;

public interface IPaymentService
{
    Task<ServiceResponse> CreatePayment(PaymentRequest requestBody, CancellationToken cancellationToken = default);
    Task<ServiceResponse> HandleWebhook(string transaction, CancellationToken cancellationToken = default);
}