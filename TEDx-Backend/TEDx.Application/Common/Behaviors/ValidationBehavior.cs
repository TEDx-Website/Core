using FluentValidation;
using MediatR;
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

        var errors = failures
            .Select(f => Error.Validation(Errors.ValidationError.Code, f.ErrorMessage, f.PropertyName));

        var responseType = typeof(TResponse);
        if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(Result<>))
            return await next();

        var failure = typeof(Result<>)
            .MakeGenericType(responseType.GetGenericArguments()[0])
            .GetMethod("Failure")!
            .Invoke(null, [errors]);

        return (TResponse)failure!;
    }
}
