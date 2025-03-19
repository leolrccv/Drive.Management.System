using FluentValidation;

namespace Application.Commands.v1;
public class ConvertFileCommandValidator : AbstractValidator<ConvertFileCommand>
{
    //TODO: adicionar validator de file != null e length > 0
}