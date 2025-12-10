using FluentValidation;
using LateralCMS.Application.DTOs;

public class CmsEventDtoValidator : AbstractValidator<CmsEventDto>
{
    public CmsEventDtoValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => new[] { "publish", "unpublish", "delete" }.Contains(t.ToLowerInvariant()))
            .WithMessage("Type must be publish, unpublish, or delete.");

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Timestamp)
            .NotEmpty();

        When(x => x.Type != null && (x.Type.Equals("publish", StringComparison.OrdinalIgnoreCase) || x.Type.Equals("unpublish", StringComparison.OrdinalIgnoreCase)), () =>
        {
            RuleFor(x => x.Version)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.Payload)
                .NotEmpty();
        });
    }
}
