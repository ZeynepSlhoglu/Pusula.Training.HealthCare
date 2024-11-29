using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Addresses;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Patients;

[RemoteService]
[Area("app")]
[ControllerName("Patient")]
[Route("api/app/patients")]
public class IPatientController(IPatientAppService patientAppService) : HealthCareController, IPatientAppService
{
    [HttpGet("all")]
    public virtual Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input) =>
        patientAppService.GetListAsync(input);

    [HttpGet("with-identity-or-pasport/{number}")]
    public virtual Task<PatientDto> GetAsync(string number) => patientAppService.GetAsync(number);

    [HttpGet("all/navigation-properties")]
    public Task<PagedResultDto<PatientWithNavigationPropertiesDto>> GetNavigationPropertiesListAsync(
        GetPatientsInput input
    ) =>
        patientAppService.GetNavigationPropertiesListAsync(input);

    [HttpGet("address/{id}")]
    public Task<List<AddressWithNavigationPropertiesDto>> GetAddressNavigationPropertiesListAsync(Guid patientId) =>
        patientAppService.GetAddressNavigationPropertiesListAsync(patientId);

    [HttpGet("{id}")]
    public virtual Task<PatientDto> GetAsync(Guid id) => patientAppService.GetAsync(id);

    [HttpGet("{id}/navigation-properties")]
    public virtual Task<PatientWithNavigationPropertiesDto> GetNavigationPropertiesAsync(Guid id) =>
        patientAppService.GetNavigationPropertiesAsync(id);

    [HttpPost]
    public virtual Task<PatientDto> CreateAsync(PatientCreateDto input) => patientAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public virtual Task<PatientDto> UpdateAsync(Guid id, PatientUpdateDto input) =>
        patientAppService.UpdateAsync(id, input);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id) => patientAppService.DeleteAsync(id);

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(PatientExcelDownloadDto input) =>
        patientAppService.GetListAsExcelFileAsync(input);

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync() => patientAppService.GetDownloadTokenAsync();

    [HttpGet("passport-exist/{passportNumber}")]
    public async Task<bool>
        PassportNumberExistsAsync(string passportNumber, [FromQuery] Guid? exludePatientId = null) =>
        await patientAppService.PassportNumberExistsAsync(passportNumber);

    [HttpGet("identity-exist/{passportNumber}")]
    public async Task<bool>
        IdentityNumberExistsAsync(string identityNumber, [FromQuery] Guid? exludePatientId = null) =>
        await patientAppService.IdentityNumberExistsAsync(identityNumber);

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> patientIds) => patientAppService.DeleteByIdsAsync(patientIds);

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetPatientsInput input) => patientAppService.DeleteAllAsync(input);
}