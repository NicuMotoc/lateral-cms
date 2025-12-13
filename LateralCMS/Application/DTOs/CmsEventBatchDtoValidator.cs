using FluentValidation;

namespace LateralCMS.Application.DTOs;

public class CmsEventBatchDtoValidator : AbstractValidator<CmsEventBatchDto>
{
    public CmsEventBatchDtoValidator()
    {
        RuleForEach(x => x.Events).SetValidator(new CmsEventDtoValidator());
    }
}
