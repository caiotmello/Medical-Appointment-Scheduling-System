namespace MedicalSystem.Application.Dtos.User
{
    public class UserTokenRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
