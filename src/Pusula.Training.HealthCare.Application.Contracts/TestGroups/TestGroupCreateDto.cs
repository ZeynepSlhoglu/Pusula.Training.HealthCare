﻿using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.TestGroups;

public class TestGroupCreateDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}
