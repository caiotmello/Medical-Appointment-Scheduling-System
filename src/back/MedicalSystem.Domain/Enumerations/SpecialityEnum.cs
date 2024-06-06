using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Domain.Enumerations
{
    public enum SpecialityEnum : int
    {
        Cardiologist = 1,
        Pediatrician = 2,
        Otolaryngologist = 3,
        GeneralPractitioner = 4,
    }
}
