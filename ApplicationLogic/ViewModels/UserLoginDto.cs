﻿using System.ComponentModel.DataAnnotations;

namespace ApplicationLogic.ViewModels
{
    public class UserLoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(8)]
        public required string Password { get; set; }
    }
}
