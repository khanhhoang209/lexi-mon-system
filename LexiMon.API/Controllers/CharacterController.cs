using System.Security.Claims;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/characters")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _service;
    private readonly IAzureBlobService _blob;
    private readonly string _defaultContainer;
    public CharacterController(ICharacterService service, IConfiguration configuration, IAzureBlobService blob)
    {
        _service = service;
        _blob = blob;
        _defaultContainer = configuration["Azure:BlobStorageSettings:DefaultContainer"] ?? "images";
    }

    /// <summary>
    /// Updates a character's information, including equipment images.
    /// </summary>
    /// <param name="id">The character's unique identifier.</param>
    /// <param name="requestForm">Form data containing character details and equipment files.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Result of the update operation.</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromForm] CharacterFormDto requestForm,
        CancellationToken cancellationToken = default)
    {  
        var userId= User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found");
        
        string? armorUrl = null;
        if (requestForm.Armor is { Length: > 0 })
        {
            using var s = requestForm.Armor.OpenReadStream();
            armorUrl = await _blob.UploadAsync(s, requestForm.Armor.FileName, _defaultContainer);
        }
        string? bootUrl = null;
        if (requestForm.Boot is { Length: > 0 })
        {   
            using var s = requestForm.Boot.OpenReadStream();
            bootUrl = await _blob.UploadAsync(s, requestForm.Boot.FileName, _defaultContainer);
        }
        string? helmetUrl = null;
        if (requestForm.Helmet is { Length: > 0 })
        {
            using var s = requestForm.Helmet.OpenReadStream();
            helmetUrl = await _blob.UploadAsync(s, requestForm.Helmet.FileName, _defaultContainer);
        }
        string? weaponUrl = null;
        if (requestForm.Weapon is { Length: > 0 })
        {
            using var s = requestForm.Weapon.OpenReadStream();
            weaponUrl = await _blob.UploadAsync(s, requestForm.Weapon.FileName, _defaultContainer);
        }

        var request = new CharacterRequestDto()
        {
            Name = requestForm.Name,
            Level = requestForm.Level,
            Exp = requestForm.Exp,
            ArmorUrl = armorUrl,
            BootUrl = bootUrl,
            HelmetUrl = helmetUrl,
            WeaponUrl = weaponUrl
        };
        
        var serviceResponse = await _service.UpdateCharacterAsync(userId, id, request, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
        
    /// <summary>
    /// Retrieves the character information for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Character information for the user.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IResult> GetByUserAsync(CancellationToken cancellationToken = default)
    {
        var userId= User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found");
        var serviceResponse = await _service.GetCharacterByUserAsync(userId, cancellationToken);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }
        return TypedResults.BadRequest(serviceResponse);
    }
}