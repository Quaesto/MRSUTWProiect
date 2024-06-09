using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "The Email is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "The Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "The Confirm Password  is Required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }
        [Required(ErrorMessage = "The User Name is Required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}