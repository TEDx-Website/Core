using System.Collections.Concurrent;
using System.Reflection;
using TEDx.Domain.Common;

namespace TEDx.Application.Common.Behaviors;
// This class is responsible for creating failure responses for requests that are routed through the validation/authorization behaviors.
internal static class FailureResponseFactory
{
    private static readonly ConcurrentDictionary<Type, MethodInfo?> FailureMethods = new();

    internal static bool CanCreate<TResponse>() =>
        typeof(TResponse) == typeof(Error) || ResolveFailureMethod(typeof(TResponse)) is not null;

    internal static TResponse Create<TResponse>(Error error) =>
        Create<TResponse>([error]);

    internal static TResponse Create<TResponse>(IReadOnlyList<Error> errors)
    {
        if (errors.Count == 0)
            throw new ArgumentException("At least one error is required.", nameof(errors));

        var responseType = typeof(TResponse);

        if (responseType == typeof(Error))
            return (TResponse)(object)errors[0];

        var failure = ResolveFailureMethod(responseType)
            ?? throw new InvalidOperationException(
                $"Cannot build a failure response for '{responseType.Name}'. Requests routed through " +
                $"the validation/authorization behaviors must return Result<T> or Error.");

        return (TResponse)failure.Invoke(null, [errors])!;
    }
    private static MethodInfo? ResolveFailureMethod(Type responseType) =>
        FailureMethods.GetOrAdd(responseType, static type =>
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Result<>))
                return null;

            return type.GetMethod(
                nameof(Result<object>.Failure),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(IReadOnlyList<Error>)],
                modifiers: null);
        });
}
