﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Appointments;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Appointments.Default)]
public class AppointmentsAppService(
    IAppointmentRepository appointmentRepository,
    AppointmentManager appointmentManager,
    IDistributedCache<AppointmentDownloadTokenCacheItem, string> downloadTokenCache,
    IAppointmentTypeRepository appointmentTypeRepository,
    IPatientRepository patientRepository
) : HealthCareAppService, IAppointmentsAppService
{
#region GetList

    public virtual async Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
    {
        var totalCount = await appointmentRepository.GetCountAsync(
            input.FilterText, input.StartTime, input.EndTime, input.Note, input.Statuses, input.AppointmentTypeId,
            input.DepartmentId, input.DoctorId, input.PatientId
        );
        var items = await appointmentRepository.GetListAsync(
            input.FilterText, input.StartTime, input.EndTime, input.Note, input.Statuses, input.AppointmentTypeId,
            input.DepartmentId, input.DoctorId, input.PatientId, input.Sorting, input.MaxResultCount, input.SkipCount
        );

        return new PagedResultDto<AppointmentDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(items)
        };
    }

#endregion

#region GetWithNavigationProperties

    public async Task<List<AppointmentWithNavigationPropertiesDto>> GetPatientWaitingAppointmentsAsync(Guid patientId)
    {
        var items = await appointmentRepository.GetListWithNavigationPropertiesAsync(
            patientId: patientId, startTime: DateTime.Now
        );
        return ObjectMapper
            .Map<List<AppointmentWithNavigationProperties>, List<AppointmentWithNavigationPropertiesDto>>(items);
    }    

    public virtual async Task<AppointmentWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) =>
        ObjectMapper.Map<AppointmentWithNavigationProperties, AppointmentWithNavigationPropertiesDto>
            (await appointmentRepository.GetWithNavigationPropertiesAsync(id));

    public async Task<List<AppointmentWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(
        GetAppointmentsInput input
    )
    {
        var items = await appointmentRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText, input.StartTime, input.EndTime, input.Note, input.Statuses, input.AppointmentTypeId,
            input.DepartmentId, input.DoctorId, input.PatientId, input.Sorting, input.MaxResultCount, input.SkipCount
        );

        var count = await appointmentRepository.GetCountAsync(
            input.FilterText, input.StartTime, input.EndTime, input.Note, input.Statuses, input.AppointmentTypeId,
            input.DepartmentId, input.DoctorId, input.PatientId
        );

        return ObjectMapper
            .Map<List<AppointmentWithNavigationProperties>, List<AppointmentWithNavigationPropertiesDto>>(items);
    }

#endregion

#region Get

    public virtual async Task<AppointmentDto> GetAsync(Guid id) =>
        ObjectMapper.Map<Appointment, AppointmentDto>(await appointmentRepository.GetAsync(id));

#endregion

#region GetAppointmentTypeLookup

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAppointmentTypeLookupAsync(LookupRequestDto input)
    {
        var query = (await appointmentTypeRepository.GetQueryableAsync())
            .WhereIf(
                !string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name != null && x.Name.Contains(input.Filter!)
            );

        var lookupData =
            await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppointmentType>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<AppointmentType>, List<LookupDto<Guid>>>(lookupData)
        };
    }

#endregion

#region GetPatientLookup

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input)
    {
        var query = (await patientRepository.GetQueryableAsync())
            .WhereIf(
                !string.IsNullOrWhiteSpace(input.Filter),
                x => x.FirstName != null && x.FirstName.Contains(input.Filter!)
            );

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Patient>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Patient>, List<LookupDto<Guid>>>(lookupData)
        };
    }

#endregion

#region Delete

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteAsync(Guid id) => await appointmentRepository.DeleteAsync(id);

#endregion

#region Create

    [Authorize(HealthCarePermissions.Appointments.Create)]
    public virtual async Task<AppointmentDto> CreateAsync(AppointmentCreateDto input)
    {
        var appointment = await appointmentManager.CreateAsync(
            input.AppointmentTypeId, input.DepartmentId, input.DoctorId, input.PatientId, input.StartTime,
            input.EndTime, input.Status, input.Notes
        );

        return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
    }

#endregion

#region Update

    [Authorize(HealthCarePermissions.Appointments.Edit)]
    public virtual async Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
    {
        var appointment = await appointmentManager.UpdateAsync(
            id, input.AppointmentTypeId, input.DepartmentId, input.DoctorId, input.PatientId, input.StartTime,
            input.EndTime, input.Status, input.Notes
        );

        return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
    }

    [Authorize(HealthCarePermissions.Appointments.Edit)]
    public virtual async Task<AppointmentDto> UpdateDateAsync(Guid id, DateTime startTime, DateTime endTime)
    {
        var appointment = await appointmentManager.UpdateDateAsync(id, startTime, endTime);

        return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
    }

    #endregion

#region GetListAsExcelFile

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var appointments = await appointmentRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText, input.StartTime, input.EndTime, input.Notes, input.Statuses, input.AppointmentTypeId,
            input.DepartmentId, input.DoctorId, input.PatientId
        );
        var items = appointments.Select(
            item => new
            {
                item.Appointment.StartTime,
                item.Appointment.EndTime,
                item.Appointment.Status,
                item.Appointment.Note,
                AppointmentType = item.AppointmentType?.Name,
                Department = item.Department?.Name,
                Doctor = item.Doctor?.FirstName,
                Patient = item.Patient?.FirstName
            }
        );
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(items);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(
            memoryStream, "Appointments.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );
    }

    #endregion    

#region DeleteById

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> appointmentIds) =>
        await appointmentRepository.DeleteManyAsync(appointmentIds);

#endregion

#region DeleteAll

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteAllAsync(GetAppointmentsInput input) =>
        await appointmentRepository.DeleteAllAsync(
            input.FilterText, input.StartTime, input.EndTime, input.Note, input.Statuses, input.AppointmentTypeId,
            input.DepartmentId, input.DoctorId, input.PatientId
        );

#endregion

#region GetDownloadToken

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new AppointmentDownloadTokenCacheItem { Token = token },
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) }
        );

        return new DownloadTokenResultDto { Token = token };
    }

#endregion

#region GetDoctorAppointment

    public async Task<List<AppointmentDto>> GetListAppointmentsAsync(Guid id) =>
        ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(
            await appointmentRepository.GetListAsync(startTime: DateTime.Now, doctorId: id)
        );

    public async Task<PagedResultDto<AppointmentWithNavigationPropertiesDto>>
        GetDoctorAppointmentListWithNavigationPropertiesAsync(GetDoctorAppointmentListInput input)
    {
        var items = await appointmentRepository.GetDoctorAppointmentListWithNavigationPropertiesAsync(
            input.DoctorId, input.StartTime, input.EndTime, input.Status, input.Sorting, input.MaxResultCount,
            input.SkipCount
        );
        var count = await appointmentRepository.GetCountForDoctorAppointmentListAsync(
            input.DoctorId, input.StartTime, input.EndTime, input.Status
        );

        return new PagedResultDto<AppointmentWithNavigationPropertiesDto>(
            count,
            ObjectMapper.Map<List<AppointmentWithNavigationProperties>, List<AppointmentWithNavigationPropertiesDto>>(
                items
            )
        );
    }

#endregion


}