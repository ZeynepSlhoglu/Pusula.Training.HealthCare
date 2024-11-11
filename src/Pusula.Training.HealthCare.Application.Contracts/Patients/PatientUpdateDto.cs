using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Patients;

public class PatientUpdateDto : IHasConcurrencyStamp
{
    [Required]
    [StringLength(PatientConsts.FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(PatientConsts.LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [Required] public DateTime BirthDate { get; set; }

    [Required]
    [StringLength(PatientConsts.IdentityNumberMaxLength)]
    public string IdentityNumber { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(PatientConsts.EmailAddressMaxLength)]
    public string EmailAddress { get; set; } = null!;

    [Required]
    [Phone]
    [StringLength(PatientConsts.PhoneNumberMaxLength)]
    public string MobilePhoneNumber { get; set; } = null!;

    [Phone]
    [StringLength(PatientConsts.PhoneNumberMaxLength)]
    public string? HomePhoneNumber { get; set; }

    [Required] public EnumGender Gender { get; set; }

    [Required] public EnumBloodType BloodType { get; set; }

    [Required] public EnumMaritalStatus MaritalStatus { get; set; }

    [Required]
    [StringLength(int.MaxValue)]
    public string Address { get; set; } = null!;

    public Guid DistrictId { get; set; }
    public Guid CountryId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}