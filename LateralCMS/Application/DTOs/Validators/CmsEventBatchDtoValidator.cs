using FluentValidation;

namespace LateralCMS.Application.DTOs.Validators;

public class CmsEventBatchDtoValidator : AbstractValidator<CmsEventBatchDto>
{
    public CmsEventBatchDtoValidator()
    {
        RuleForEach(x => x.Events).SetValidator(new CmsEventDtoValidator());
    }
}
