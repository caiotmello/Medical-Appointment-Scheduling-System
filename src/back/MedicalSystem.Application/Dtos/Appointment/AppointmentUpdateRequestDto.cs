﻿using MedicalSystem.Domain.Enumerations;

namespace MedicalSystem.Application.Dtos.Appointment
{
    public class AppointmentUpdateRequestDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public AppoitmentStatusEnum Status { get; set; }

    }
}
