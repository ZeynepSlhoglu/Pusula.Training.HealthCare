using System.Collections.Generic;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.PatientNotes;
using Pusula.Training.HealthCare.PatientTypes;

namespace Pusula.Training.HealthCare.Patients;

public class PatientWithNavigationPropertiesDto
{
    public PatientDto Patient { get; set; }
    public CountryDto Country { get; set; }
    public PatientTypeDto PatientType { get; set; }
    public IEnumerable<PatientNoteDto> PatientNotes { get; set; }
}