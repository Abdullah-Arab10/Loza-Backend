﻿using System.ComponentModel.DataAnnotations;

namespace Loza.Models.DTOs
{
    public class UserLoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
