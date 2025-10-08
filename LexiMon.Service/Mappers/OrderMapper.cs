using LexiMon.Repository.Domains;
using LexiMon.Repository.Enum;
using LexiMon.Service.Models.Requests;

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
}