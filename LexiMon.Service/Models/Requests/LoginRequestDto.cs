﻿using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class LoginRequestDto
{
    [EmailAddress(ErrorMessage = "Vui lòng nhập email!")]
    [Required(ErrorMessage = "Vui lòng nhập email!")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
    public string Password { get; set; } = null!;
}