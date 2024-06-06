namespace MedicalSystem.Domain.Entities
{
    public class Patient : ApplicationUser
    {
        public string? CPF {  get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
