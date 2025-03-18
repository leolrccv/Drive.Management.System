using ErrorOr;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;
public class ValidationHandler<TRequest, TResponse>(IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
            return await next();

        var validation = await _validator.ValidateAsync(request, cancellationToken);

        if (validation.IsValid)
            return await next();

        return (dynamic)validation.Errors.ConvertAll(validationFailure => Error.Validation(validationFailure.ErrorMessage));
    }
}