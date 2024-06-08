using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class EditModel
    {

        public string Name { get; set; }

        public string Address { get; set; }
        [Required(ErrorMessage = "The First Name is Required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "The Email is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Image")]
        public string ProfileImage { get; set; }
    }
}