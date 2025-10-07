using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class ItemMapper
{
    public static ItemResponseDto ToItemResponse(this Item item)
    {
        return new ItemResponseDto()
        {
            ItemId = item.Id,
            ItemName = item.Name,
            IsPremium = item.IsPremium,
            CategoryId = item.CategoryId,
            CategoryName = item.Category!.Name,
            Price = item.Price,
            Coin = item.Coin,
            Description = item.Description,
            ImageUrl = item.ImageUrl,
            IsActive = item.Status
        };
    }

    public static Item ToItem(this ItemRequestDto request)
    {
        return new Item()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsPremium = request.IsPremium,
            Coin = request.Coin,
            Price = request.Price,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Status = true,
            CategoryId = request.CategoryId,
            CreatedAt = DateTimeOffset.Now
        };
    }

    public static void UpdateItem(this Item item, ItemRequestDto request)
    {
        item.Name = request.Name;
        item.IsPremium = request.IsPremium;
        item.Description = request.Description;
        item.Coin = request.Coin;
        item.Price = request.Price;
        item.ImageUrl = request.ImageUrl;
        item.CategoryId = request.CategoryId;
    }
}