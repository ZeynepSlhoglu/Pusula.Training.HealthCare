﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.AppointmentReports
{
    public class AppointmentReportUpdateDto : IHasConcurrencyStamp
    {
        public DateTime ReportDate { get; set; }
        public string? PriorityNotes { get; set; } 
        public string? DoctorNotes { get; set; } 
        public Guid AppointmentId { get; set; }

        public string? ConcurrencyStamp { get; set; }
    }
}