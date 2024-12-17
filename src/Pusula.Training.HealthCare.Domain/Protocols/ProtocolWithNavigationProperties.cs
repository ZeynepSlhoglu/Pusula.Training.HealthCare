using System.Collections.Generic;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypeActions;
using Pusula.Training.HealthCare.ProtocolTypes;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithNavigationProperties
{
    public Protocol Protocol { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public ProtocolType ProtocolType { get; set; } = null!;
    public ProtocolTypeAction ProtocolTypeAction { get; set; } = null!;
}