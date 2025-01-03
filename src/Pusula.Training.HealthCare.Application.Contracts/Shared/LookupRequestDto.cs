using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Shared;

public class LookupRequestDto : PagedResultRequestDto
{
    public string? Filter { get; set; }

    public LookupRequestDto()
    {
        MaxResultCount = MaxMaxResultCount;
    }
}