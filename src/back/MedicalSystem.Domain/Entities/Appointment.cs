
using MedicalSystem.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MedicalSystem.Domain.Entities
{
    public class Appointment : EntityBase
    {
        //public int PatientId { get; set; }
        //public Patient Patient { get; set; }
        //public int DoctorId { get; set; }
        //public Doctor Doctor { get; set; }
        [Required]
        public string PatientId { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public AppoitmentStatusEnum Status { get; set; }
    }
}


//[Required]
//[Display(Name = "Date for Appointment")]
//[DataType(DataType.Date)]
//[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
////[MyAppointmentDateValidation(ErrorMessage = "Are you creating an appointment for the past?")]
//public DateTime AppointmentDate { get; set; }

//[DataType(DataType.Time)]
//public DateTime Time { get; set; }
