using FluentValidation;
using MediatR;
using TEDx.Application.Common.Errors;
using TEDx.Domain.Common;

namespace TEDx.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .SelectMany(v => v.Validate(context).Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        if (!FailureResponseFactory.CanCreate<TResponse>())
            return await next();

        var errors = failures
            .Select(f => Error.Validation(
                IsCustomCode(f.ErrorCode)
                    ? f.ErrorCode
                    : CommonErrors.ValidationError.Code,
                f.ErrorMessage,
                f.PropertyName))
            .ToList();

        return FailureResponseFactory.Create<TResponse>(errors);
    }

    // FluentValidation defaults ErrorCode to the validator's type name
    // ("NotEmptyValidator"); anything else was set with WithErrorCode.
    private static bool IsCustomCode(string? code)
        => !string.IsNullOrWhiteSpace(code)
           && !code.EndsWith("Validator", StringComparison.Ordinal);
}
