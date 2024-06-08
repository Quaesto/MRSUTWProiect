using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "The User Name is Required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "The Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}