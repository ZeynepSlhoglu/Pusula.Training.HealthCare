using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Addresses;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.Patients;

public interface IPatientAppService : IApplicationService
{
    Task<PatientDto> GetAsync(string number);
    Task<PatientDto> GetAsync(Guid id);
    Task<PatientWithNavigationPropertiesDto> GetNavigationPropertiesAsync(Guid id);

    Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input);
    Task<PagedResultDto<PatientWithNavigationPropertiesDto>> GetNavigationPropertiesListAsync(GetPatientsInput input);
    Task<List<AddressWithNavigationPropertiesDto>> GetAddressNavigationPropertiesListAsync(Guid patientId);

    Task DeleteAsync(Guid id);

    Task<PatientDto> CreateAsync(PatientCreateDto input);

    Task<PatientDto> UpdateAsync(Guid id, PatientUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(PatientExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> patientIds);

    Task DeleteAllAsync(GetPatientsInput input);

    Task<DownloadTokenResultDto> GetDownloadTokenAsync();

    Task<bool> PassportNumberExistsAsync(string passportNumber, Guid? excludePatientId = null);
    Task<bool> IdentityNumberExistsAsync(string identityNumber, Guid? excludePatientId = null);
}