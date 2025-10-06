using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;
[ApiController]
[Route("api/enemies")]
public class EnemiesController : ControllerBase
{
     private readonly IEnemyService _service;
     private readonly IAzureBlobService _blob;
     private readonly string _defaultContainer;
     
    public EnemiesController(IEnemyService service, IAzureBlobService blob, IConfiguration configuration)
    {
        _service = service;
        _blob = blob;
        _defaultContainer = configuration["Azure:BlobStorageSettings:DefaultContainer"] ?? "images";
    }
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IResult> CreateAsync(
        [FromForm]  EnemyFormDto request,
        CancellationToken cancellationToken = default)
    {
        string? imageUrl = null;
        if (request.ImageUrl is { Length: > 0 })
        {
            using var s = request.ImageUrl.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, request.ImageUrl.FileName, _defaultContainer);
        }

        string? animationAttackUrl = null;
        if (request.AnimationAttackUrl is { Length: > 0 })
        {
            using var s = request.AnimationAttackUrl.OpenReadStream();
            animationAttackUrl = await _blob.UploadAsync(s, request.AnimationAttackUrl.FileName, _defaultContainer);
        }
        
        string? animationMove = null;
        if (request.AnimationMoveUrl is { Length: > 0 })
        {
            using var s = request.AnimationMoveUrl.OpenReadStream();
            animationMove = await _blob.UploadAsync(s, request.AnimationMoveUrl.FileName, _defaultContainer);
        }
        string? helmer = null;
        if (request.HelmerUrl is { Length: > 0 })
        {
            using var s = request.HelmerUrl.OpenReadStream();
            helmer = await _blob.UploadAsync(s, request.HelmerUrl.FileName, _defaultContainer);
        }
        
        string? armor = null;
        if (request.ArmorUrl is { Length: > 0 })
        {
            using var s = request.ArmorUrl.OpenReadStream();
            armor = await _blob.UploadAsync(s, request.ArmorUrl.FileName, _defaultContainer);
        }
        
        string? boot = null;
        if (request.BootUrl is { Length: > 0 })
        {
            using var s = request.BootUrl.OpenReadStream();
            boot = await _blob.UploadAsync(s, request.BootUrl.FileName, _defaultContainer);
        }
        
        string? weapon = null;
        if (request.WeaponUrl is { Length: > 0 })
        {
            using var s = request.WeaponUrl.OpenReadStream();
            weapon = await _blob.UploadAsync(s, request.WeaponUrl.FileName, _defaultContainer);
        }
        
        var requestDto = new EnemyRequestDto
        {
            Name = request.Name,
            ImageUrl = imageUrl,
            AnimationAttackUrl = animationAttackUrl,
            AnimationMoveUrl = animationMove,
            HelmerUrl = helmer,
            ArmorUrl = armor,
            BootUrl = boot,
            WeaponUrl = weapon,
            Quantity = request.Quantity,
            Description = request.Description,
            EnemyLevelId = request.EnemyLevelId
        };
        
        var serviceResponse = await _service.CreateEnemyAsync(requestDto, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Created($"/api/animation-types/{serviceResponse.Data}", serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromForm]  EnemyFormDto request,
        CancellationToken cancellationToken = default)
    {
        
            string? imageUrl = null;
        if (request.ImageUrl is { Length: > 0 })
        {
            using var s = request.ImageUrl.OpenReadStream();
            imageUrl = await _blob.UploadAsync(s, request.ImageUrl.FileName, _defaultContainer);
        }

        string? animationAttackUrl = null;
        if (request.AnimationAttackUrl is { Length: > 0 })
        {
            using var s = request.AnimationAttackUrl.OpenReadStream();
            animationAttackUrl = await _blob.UploadAsync(s, request.AnimationAttackUrl.FileName, _defaultContainer);
        }
        
        string? animationMove = null;
        if (request.AnimationMoveUrl is { Length: > 0 })
        {
            using var s = request.AnimationMoveUrl.OpenReadStream();
            animationMove = await _blob.UploadAsync(s, request.AnimationMoveUrl.FileName, _defaultContainer);
        }
        string? helmer = null;
        if (request.HelmerUrl is { Length: > 0 })
        {
            using var s = request.HelmerUrl.OpenReadStream();
            helmer = await _blob.UploadAsync(s, request.HelmerUrl.FileName, _defaultContainer);
        }
        
        string? armor = null;
        if (request.ArmorUrl is { Length: > 0 })
        {
            using var s = request.ArmorUrl.OpenReadStream();
            armor = await _blob.UploadAsync(s, request.ArmorUrl.FileName, _defaultContainer);
        }
        
        string? boot = null;
        if (request.BootUrl is { Length: > 0 })
        {
            using var s = request.BootUrl.OpenReadStream();
            boot = await _blob.UploadAsync(s, request.BootUrl.FileName, _defaultContainer);
        }
        
        string? weapon = null;
        if (request.WeaponUrl is { Length: > 0 })
        {
            using var s = request.WeaponUrl.OpenReadStream();
            weapon = await _blob.UploadAsync(s, request.WeaponUrl.FileName, _defaultContainer);
        }
        
        var requestDto = new EnemyRequestDto
        {
            Name = request.Name,
            ImageUrl = imageUrl,
            AnimationAttackUrl = animationAttackUrl,
            AnimationMoveUrl = animationMove,
            HelmerUrl = helmer,
            ArmorUrl = armor,
            BootUrl = boot,
            WeaponUrl = weapon,
            Quantity = request.Quantity,
            Description = request.Description,
            EnemyLevelId = request.EnemyLevelId
        };
        
        var serviceResponse = await _service.UpdateEnemyAsync(id, requestDto, cancellationToken);
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
        var serviceResponse = await _service.DeleteEnemyAsync(id, cancellationToken);
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
        var serviceResponse = await _service.GetEnemyByIdAsync(id, cancellationToken);
        if (!serviceResponse.Succeeded)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Ok(serviceResponse);
    }

    [HttpGet]
    public async Task<IResult> GetAllAsync(
        [FromQuery] GetBaseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var serviceResponse = await _service.GetEnemiesAsync(request, cancellationToken);
        return Results.Ok(serviceResponse);
        
    }
}