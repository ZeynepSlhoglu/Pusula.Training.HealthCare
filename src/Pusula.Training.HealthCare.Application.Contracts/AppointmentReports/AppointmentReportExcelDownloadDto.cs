﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.AppointmentReports
{
    public class AppointmentReportExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;
        public string? FilterText { get; set; }

        public DateTime? ReportDate { get; set; }
        public string? PriorityNotes { get; set; } = null!;
        public string? DoctorNotes { get; set; } = null!;
        public Guid? AppointmentId { get; set; }

        public AppointmentReportExcelDownloadDto() { }
    }
}