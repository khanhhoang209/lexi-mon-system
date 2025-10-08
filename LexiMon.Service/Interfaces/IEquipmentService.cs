using LexiMon.Service.ApiResponse;
using LexiMon.Service.Implements;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Interfaces;

public interface IEquipmentService
{
    Task<PaginatedResponse<EquipmentResponseDto>> GetEquipmentsAsync(
        string userId,
        GetEquipmentRequest request,
        CancellationToken cancellationToken = default);
}