using System.ComponentModel.DataAnnotations;

namespace MRSTWEb.Models
{
    public class ResetPasswordViewModel
    {
        public string Code { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; }
    }
}