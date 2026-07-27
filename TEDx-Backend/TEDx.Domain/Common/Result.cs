namespace TEDx.Domain.Common
{
    public sealed class Result<TValue>
    {
        private readonly TValue? _value;
        private readonly List<Error> _errors;

        public bool IsSuccess { get; }
        public bool IsError => !IsSuccess;

        public TValue Value => _value!;
        public List<Error> Errors => _errors;
        public Error FirstError => _errors.Count > 0 ? _errors[0] : Error.None;

        public Result(TValue? value, List<Error>? errors, bool isSuccess)
        {
            if (isSuccess)
            {
                _value = value ?? throw new ArgumentNullException(nameof(value));
                _errors = [];
                IsSuccess = true;
            }
            else
            {
                if (errors is null || errors.Count == 0)
                {
                    throw new ArgumentException("Provide at least one error.", nameof(errors));
                }

                _errors = errors;
                _value = default!;
                IsSuccess = false;
            }
        }

        private Result(Error error)
        {
            _errors = [error];
            _value = default!;
            IsSuccess = false;
        }

        private Result(List<Error> errors)
        {
            if (errors is null || errors.Count == 0)
            {
                throw new ArgumentException(
                    "Cannot create a Result<TValue> from an empty collection of errors. Provide at least one error.",
                    nameof(errors));
            }

            _errors = errors;
            _value = default!;
            IsSuccess = false;
        }

        private Result(TValue value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _errors = [];
            IsSuccess = true;
        }
    }
}
