using System.ComponentModel.DataAnnotations;

namespace MRSTWEb.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "The Email is Required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}