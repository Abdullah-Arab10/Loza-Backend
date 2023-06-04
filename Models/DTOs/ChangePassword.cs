﻿using System.ComponentModel.DataAnnotations;

namespace Loza.Models.DTOs
{
    public class ChangePassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
