using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/animations")]
public class AnimationsController : ControllerBase
{
    private readonly IAnimationService _service;
    private readonly IAzureBlobService _blob;
    private readonly string _defaultContainer;
    
    public AnimationsController(IAnimationService service, IConfiguration configuration, IAzureBlobService blob)
    {
        _service = service;
        _blob = blob;
        _defaultContainer = configuration["Azure:BlobStorageSettings:DefaultContainer"] ?? "images";
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IResult> CreateAsync(
        [FromForm]  AnimationFormDto requestForm,
        CancellationToken cancellationToken = default)
    {
        string? imageUrl = null;
        if (requestForm.Image is { Length: > 0 })
        {
            using var s = requestForm.Image.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, requestForm.Image.FileName, _defaultContainer);
        }

        var request = new AnimationRequestDto()
        {
            Name = requestForm.Name,
            AnimationUrl = imageUrl,
            AnimationTypeId = requestForm.AnimationTypeId,
            ItemId = requestForm.ItemId,
        };
        
        var serviceResponse = await _service.CreateAnimationAsync(request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/animations/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromForm]  AnimationFormDto requestForm,
        CancellationToken cancellationToken = default)
    {
        string? imageUrl = null;
        if (requestForm.Image is { Length: > 0 })
        {
            using var s = requestForm.Image.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, requestForm.Image.FileName, _defaultContainer);
        }

        var request = new AnimationRequestDto()
        {
            Name = requestForm.Name,
            AnimationUrl = imageUrl,
            AnimationTypeId = requestForm.AnimationTypeId,
            ItemId = requestForm.ItemId,
        };
        
        var serviceResponse = await _service.UpdateAnimationAsync(id, request, cancellationToken);
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
        var serviceResponse = await _service.DeleteAnimationAsync(id, cancellationToken);
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
        var serviceResponse = await _service.GetAnimationByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Ok(serviceResponse);
    }

    [HttpGet]
    public async Task<IResult> GetAllAsync(
        [FromQuery] GetAnimationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetAnimationsAsync(request, cancellationToken);
        return Results.Ok(serviceResponse);
        
    }
}