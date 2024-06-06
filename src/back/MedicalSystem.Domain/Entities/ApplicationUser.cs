using MedicalSystem.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace MedicalSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public SexEnum Sex { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public UserStatusEnum  Status {  get; set; }
    }
}
