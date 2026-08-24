namespace TEDx.Domain.Common
{
    public sealed class Result<TValue>
    {
        private readonly TValue? _value; // Nullable to allow for the case where TValue is a reference type and the Result is a failure.
        private readonly IReadOnlyList<Error> _errors; 
        public bool IsSuccess { get; }
        public bool IsError => !IsSuccess;
        public TValue Value => IsSuccess //func to Return the value if the result is successful
            ? _value!
            : throw new InvalidOperationException(
                "Cannot access Value on a failed Result. Check IsSuccess/IsError and read Errors instead.");
        public IReadOnlyList<Error> Errors => _errors;
        public Error FirstError => _errors.Count > 0 ? _errors[0] : Error.None();

        private Result(TValue value)
        {
            // Reject null only when TValue cannot represent null by design.
            // Nullable value types (e.g. EventStatus?) may legitimately succeed with null,
            // meaning "no value / not provided". Reference-type nulls are still rejected
            // because a successful Result should always carry a meaningful value.
            var isNullableValueType = Nullable.GetUnderlyingType(typeof(TValue)) is not null;
            if (value is null && !isNullableValueType)
                throw new ArgumentNullException(nameof(value));
            _value = value;
            _errors = [];
            IsSuccess = true;
        }
        // Reject null or empty errors collection to ensure that a failure Result always has at least one error.
        private Result(IReadOnlyList<Error> errors)
        {
            if (errors is null || errors.Count == 0)
            {
                throw new ArgumentException(
                    "Cannot create a Result<TValue> from an empty collection of errors. Provide at least one error.",
                    nameof(errors));
            }

            _errors = errors;
            _value = default;
            IsSuccess = false;
        }
        // Match method to handle both success and failure cases
        public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
        {
            return IsSuccess
                ? onSuccess(Value)
                : onFailure(Errors);
        }
        // Factory methods for creating Result instances
        public static Result<TValue> Success(TValue value) => new(value);
        public static Result<TValue> Failure(Error error) => new([error]);
        public static Result<TValue> Failure(IReadOnlyList<Error> errors)
               => new(errors);

        public static implicit operator Result<TValue>(TValue value)
            => Success(value);

        public static implicit operator Result<TValue>(Error error)
            => Failure(error);

    }
}
