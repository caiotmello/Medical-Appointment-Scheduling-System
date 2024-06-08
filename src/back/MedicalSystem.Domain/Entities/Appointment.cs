
using MedicalSystem.Domain.Enumerations;
using MedicalSystem.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Domain.Entities
{
    public class Appointment : EntityBase
    {
        [Required]
        public string PatientId { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [RequiredDateTime]
        public DateTime Date { get; set; }

        [RequiredTime]
        public TimeSpan Time { get; set; }

        public AppoitmentStatusEnum Status { get; set; }

        //public Appointment(string patientId, string doctorId, DateTime date, DateTime time, AppoitmentStatusEnum status)
        //{
        //    PatientId = patientId;
        //    DoctorId = doctorId;
        //    Date = date;
        //    Time = time;
        //    Status = status;
        //}

        //public void ValidateEntity()
        //{
        //    AssertionConcern.AssertArgumentNotEmpty(PatientId, "PatientId must be informed!");
        //    AssertionConcern.AssertArgumentNotEmpty(DoctorId , "DoctorId must be informed!");

        //    AssertionConcern.AssertArgumentNotNull(Date, "Date must be informed!");
        //    AssertionConcern.AssertArgumentNotNull(Time, "Time must be informed!");
        //}


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
