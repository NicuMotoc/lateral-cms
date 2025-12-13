using System.Collections.Generic;

namespace LateralCMS.Application.DTOs;

public class CmsEventBatchDto
{
    public List<CmsEventDto> Events { get; set; } = new();
}
