using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorDto : FullAuditedEntityDto<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public int WorkingHours { get; set; }
    public Guid TitleId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid HospitalId { get; set; }
}