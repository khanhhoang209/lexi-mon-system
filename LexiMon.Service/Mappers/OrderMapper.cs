using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class OrderMapper
{
    public static Order ToOrder(this OrderRequestDto orderRequestDto, string userId, decimal? purchaseCost, decimal? coinCost)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = orderRequestDto.CourseId,
            ItemId = orderRequestDto.ItemId,
            PurchaseCost = purchaseCost,
            CoinCost = coinCost,
            PaymentStatus =  PaymentStatus.Pending
        };
    }
    
    public static OrderUserResponseDto ToOrderUserResponse(this Order order)
    {
        return new OrderUserResponseDto
        {
            Id = order.Id,
            CourseId = order.CourseId,
            ItemId = order.ItemId,
            PurchaseCost = order.PurchaseCost,
            CoinCost = order.CoinCost,
            PaidAt = order.PaidAt,
            PaymentStatus = order.PaymentStatus,
            ItemName = order.Item?.Name,
            CourseTitle = order.Course?.Title,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}