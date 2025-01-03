﻿using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.RadiologyExaminationDocuments;
using Pusula.Training.HealthCare.RadiologyExaminations;
using Pusula.Training.HealthCare.RadiologyRequests;
using System.Collections.Generic;

namespace Pusula.Training.HealthCare.RadioloyRequestItems;
public class RadiologyRequestItemWithNavigationProperties
{
    public RadiologyExamination RadiologyExamination { get; set; } = null!;
    public RadiologyRequestItem RadiologyRequestItem { get; set; } = null!;
    public RadiologyRequest RadiologyRequest { get; set; } = null!;
    public List<RadiologyExaminationDocument> RadiologyDocuments { get; set; } = new List<RadiologyExaminationDocument>();
    public Protocol Protocol { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
}

