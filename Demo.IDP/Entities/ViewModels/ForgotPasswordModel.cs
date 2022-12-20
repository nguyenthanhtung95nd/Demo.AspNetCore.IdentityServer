﻿using System.ComponentModel.DataAnnotations;

namespace Demo.IDP.Entities.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}