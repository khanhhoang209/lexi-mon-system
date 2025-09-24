﻿using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LexiMon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthsController
{
    private readonly IUserService _userService;

    public AuthsController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IResult> LoginAsync([FromBody] LoginRequestDto requestBody)
    {
        var serviceResponse = await _userService.LoginAsync(requestBody);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.Unauthorized();
    }

    [HttpPost("{role}/register")]
    public async Task<IResult> RegisterAsync(
        [FromBody] RegisterRequestDto requestBody,
        [FromRoute] string role)
    {
        var serviceResponse = await _userService.RegisterAsync(requestBody, role);
        if (serviceResponse.Succeeded)
        {
            return TypedResults.Ok(serviceResponse);
        }

        return TypedResults.BadRequest(serviceResponse);
    }
}