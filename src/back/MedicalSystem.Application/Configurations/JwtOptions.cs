using Microsoft.IdentityModel.Tokens;

namespace MedicalSystem.Application.Configurations
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public SymmetricSecurityKey IssuerSigningKey { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInMinutes { get; set; }
    }
}
