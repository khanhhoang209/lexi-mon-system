using LexiMon.Repository.Domains;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;

namespace LexiMon.Service.Mappers;

public static class AnimationMapper
{
    public static AnimationResponseDto ToAnimationResponse(this Animation animation)
    {
        return new AnimationResponseDto()
        {
            AnimationId = animation.Id,
            AnimationName = animation.Name,
            AnimationUrl = animation.AnimationUrl,
            ItemId = animation.ItemId,
            ItemName = animation.Item!.Name,
            AnimationTypeId = animation.AnimationTypeId,
            AnimationTypeName = animation.AnimationType!.Name,
            IsActive = animation.Status
        };
    }

    public static Animation ToAnimation(this AnimationRequestDto request)
    {
        return new Animation()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            AnimationTypeId = request.AnimationTypeId,
            ItemId = request.ItemId,
            AnimationUrl = request.AnimationUrl,
            Status = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static void UpdateAnimation(this Animation animation, AnimationRequestDto request)
    {
        animation.Name = request.Name;
        animation.AnimationUrl= request.AnimationUrl;
        animation.AnimationTypeId = request.AnimationTypeId;
        animation.ItemId = request.ItemId;
        animation.UpdatedAt = DateTimeOffset.UtcNow;
    }
}