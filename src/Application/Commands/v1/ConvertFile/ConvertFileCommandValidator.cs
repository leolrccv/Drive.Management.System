using Application.Extensions;
using FluentValidation;

namespace Application.Commands.v1.ConvertFile;
public class ConvertFileCommandValidator : AbstractValidator<ConvertFileCommand>
{
    public ConvertFileCommandValidator()
    {
        RuleFor(x => x.File).NotEmpty().WithMessage("File.Required");

        RuleFor(x => x.File.Length).GreaterThan(0).WithMessage("File.MinLength");

        RuleFor(x => x.File.FileName).Must(x => FileTypes.GetAllowedTypes().Contains(Path.GetExtension(x))).WithMessage("File.InvalidType");
    }
}