using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Application.Dtos.User
{
    public class UserLoginRequestDto
    {
        [EmailAddress(ErrorMessage = "Please provide a valid email.")]
        [DefaultValue("example@example.com")]
        [Required(ErrorMessage = "The email is a mandatory field.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is a mandatory field.")]
        [DefaultValue("Passaword")]
        public string Password { get; set; }
    }
}
