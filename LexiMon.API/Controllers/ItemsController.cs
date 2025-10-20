using System.Security.Claims;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _service;
    private readonly IAzureBlobService _blob;
    private readonly string _defaultContainer;

    public ItemsController(IItemService service, IAzureBlobService blob, IConfiguration configuration)
    {
        _service = service;
        _blob = blob;
        _defaultContainer = configuration["Azure:BlobStorageSettings:DefaultContainer"] ?? "images";
    }

      [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IResult> CreateAsync(
        [FromForm]  ItemFormDto requestForm,
        CancellationToken cancellationToken = default)
    {
        string? imageUrl = null;
        if (requestForm.Image is { Length: > 0 })
        {
            using var s = requestForm.Image.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, requestForm.Image.FileName, _defaultContainer);
        }

        var request = new ItemRequestDto()
        {
            Name = requestForm.Name,
            ImageUrl = imageUrl,
            Description = requestForm.Description,
            Coin = requestForm.Coin,
            Price = requestForm.Price,
            CategoryId = requestForm.CategoryId,
            IsPremium = requestForm.IsPremium
        };

        var serviceResponse = await _service.CreateItemAsync(request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/items/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromForm]  ItemFormDto requestForm,
        CancellationToken cancellationToken = default)
    {
        string? imageUrl = null;
        if (requestForm.Image is { Length: > 0 })
        {
            using var s = requestForm.Image.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, requestForm.Image.FileName, _defaultContainer);
        }

        var request = new ItemRequestDto()
        {
            Name = requestForm.Name,
            ImageUrl = imageUrl,
            Description = requestForm.Description,
            Coin = requestForm.Coin,
            Price = requestForm.Price,
            CategoryId = requestForm.CategoryId,
            IsPremium = requestForm.IsPremium
        };

        var serviceResponse = await _service.UpdateItemAsync(id, request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.DeleteItemAsync(id, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(serviceResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetItemByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Ok(serviceResponse);
    }

    [HttpGet]
    public async Task<IResult> GetAllAsync(
        [FromQuery] GetItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetItemsAsync(request, cancellationToken);
        return Results.Ok(serviceResponse);

    }
    [HttpGet("shop")]
    [Authorize]
    public async Task<IResult> GetShopItemsAsync(
        [FromQuery] GetItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");
        var serviceResponse = await _service.GetShopItemsAsync(userId, request, cancellationToken);
        return Results.Ok(serviceResponse);

    }
}