using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Application.Dtos.User
{
    public class UserCreateRequestDto
    {
        [EmailAddress(ErrorMessage = "Please provide a valid email.")]
        [Required(ErrorMessage = "The email is a mandatory field.")]
        [DefaultValue("example@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is a mandatory field.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Please provide a password between 8 and 20 characters.")]
        [DefaultValue("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The password is a mandatory field.")]
        [Compare("Password", ErrorMessage = "The password must be the same!")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Please provide a password between 8 and 20 characters.")]
        [DefaultValue("Password")]
        public string PasswordConfirmation { get; set; }

        [Required(ErrorMessage = "The first name is a mandatory field.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Please provide a valid first name.")]
        [DefaultValue("Input your first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last name is a mandatory field.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Please provide a valid last name.")]
        [DefaultValue("Input your last name")]
        public string LastName { get; set; }
        
        public string Role { get; set; }

        public string? CPF { get; set; }
        public DateTime? BirthDate { get; set; }
        public string ?CRM { get; set; }
        public SpecialityEnum? Speciality { get; set; }
    }
}
