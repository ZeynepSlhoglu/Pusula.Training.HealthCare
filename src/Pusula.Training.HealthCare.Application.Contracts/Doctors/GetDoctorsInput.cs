using Volo.Abp.Application.Dtos;
using System;

namespace Pusula.Training.HealthCare.Doctors;

public class GetDoctorsInput : PagedAndSortedResultRequestDto
{
    public int? AppointmentTime { get; set; }

    public string? FilterText { get; set; }

    public string? FullName { get; set; }
    public Guid? TitleId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? HospitalId { get; set; }

    public GetDoctorsInput() { }
}