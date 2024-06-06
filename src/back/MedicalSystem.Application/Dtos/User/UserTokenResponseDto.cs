namespace MedicalSystem.Application.Dtos.User
{
    public class UserTokenResponseDto
    {
        public string Token { get; set; } = "";

        public string RefreshToken { get; set; } = "";

        public DateTime Expiration { get; set; }
    }
}
