using Volo.Abp.Application.Dtos;
using System;

namespace Pusula.Training.HealthCare.Protocols;

public class GetProtocolsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid ProtocolTypeId { get; set; }
    public EnumProtocolStatus Status { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTimeMin { get; set; }
    public DateTime? StartTimeMax { get; set; }
    public DateTime? EndTimeMin { get; set; }
    public DateTime? EndTimeMax { get; set; }

    public GetProtocolsInput() { }
}