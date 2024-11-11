﻿using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Appointments
{
    public interface IAppointmentsAppService:IApplicationService
    {
        Task<PagedResultDto<AppointmentWithNavigationPropertiesDto>> GetListAsync(GetAppointmentsInput input);
        Task<AppointmentWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
        Task<AppointmentDto> GetAsync(Guid id);
        Task<PagedResultDto<LookupDto<Guid>>> GetHospitalLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid>>> GetDoctorLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input);
        Task DeleteAsync(Guid id);
        Task<AppointmentDto> CreateAsync(AppointmentCreateDto input);
        Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> appointmentIds);

        Task DeleteAllAsync(GetAppointmentsInput input);
        Task<DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}
