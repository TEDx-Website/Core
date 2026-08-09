namespace TEDx.Domain.Common
{
    public sealed class Result<TValue>
    {
        private readonly TValue? _value;

        private readonly IReadOnlyList<Error> _errors;
        public bool IsSuccess { get; }
        public bool IsError => !IsSuccess;
        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "Cannot access Value on a failed Result. Check IsSuccess/IsError and read Errors instead.");
        public IReadOnlyList<Error> Errors => _errors;
        public Error FirstError => _errors.Count > 0 ? _errors[0] : default;

        private Result(TValue value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _errors = [];
            IsSuccess = true;
        }
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
        public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
        {
            return IsSuccess
                ? onSuccess(Value)
                : onFailure(Errors);
        }
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
