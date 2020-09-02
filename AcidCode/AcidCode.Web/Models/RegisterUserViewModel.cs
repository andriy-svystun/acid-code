using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcidCode.Web.Models
{
    public class RegisterUserViewModel
    {
        [Required]
        [Display(Name = "Your name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Login")]
        public string UserName { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsSucceeded { get; set; }
    }
}