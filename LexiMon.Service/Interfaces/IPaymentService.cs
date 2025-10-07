using LexiMon.Service.ApiResponse;
using LexiMon.Service.Models.Requests;
using Net.payOS.Types;

namespace LexiMon.Service.Interfaces;

public interface IPaymentService
{
    Task<ServiceResponse> CreatePayment(PaymentRequest requestBody, CancellationToken cancellationToken = default);
    Task<ServiceResponse> PaymentReturn(Guid orderId, CancellationToken cancellationToken = default);
    Task<ServiceResponse> PaymentCancel(Guid orderId, CancellationToken cancellationToken = default);
}