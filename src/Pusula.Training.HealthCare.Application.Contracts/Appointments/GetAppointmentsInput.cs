﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments
{
    public class GetAppointmentsInput:PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ICollection<EnumAppointmentStatus>? Statuses { get; set; }
        public string? Note { get; set; }
        public Guid? AppointmentTypeId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }

        public GetAppointmentsInput()
        {
        }
    }
}
