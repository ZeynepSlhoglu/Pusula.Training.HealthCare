﻿using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.WorkLists;

public class WorkListUpdateDto : IHasConcurrencyStamp
{
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public Guid DepartmentId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}