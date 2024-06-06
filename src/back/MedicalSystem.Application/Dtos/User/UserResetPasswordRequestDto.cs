using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Application.Dtos.User
{
    public class UserResetPasswordRequestDto
    {
        [EmailAddress(ErrorMessage = "Please provide a valid email.")]
        [DefaultValue("example@example.com")]
        [Required(ErrorMessage = "The email is a mandatory field.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is a mandatory field.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Please provide a password between 8 and 20 characters.")]
        [DefaultValue("New Passaword")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "The password is a mandatory field.")]
        [Compare("NewPassword", ErrorMessage = "The password must be the same!")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Please provide a password between 8 and 20 characters.")]
        [DefaultValue("New Passaword")]
        public string NewPasswordConfirmation { get; set; }
        
    }
}
