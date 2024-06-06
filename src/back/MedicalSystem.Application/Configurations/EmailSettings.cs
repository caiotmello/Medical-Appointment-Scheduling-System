namespace MedicalSystem.Application.Configurations
{
    public class EmailSettings
    {
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Provider { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
