using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Address { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ProfileImage { get; set; }
        public bool IsLockedOut { get; set; }

        public ICollection<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public ICollection<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();

    }
}