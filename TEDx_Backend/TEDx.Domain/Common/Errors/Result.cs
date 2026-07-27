using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.Errors
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; } //
        public T? Value { get; }
        public Error? Error { get; }

        private Result(bool isSuccess,T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }
        public static Result<T> Success(T value)
        {
            return new Result<T>(
                true,
                value,
                null);
        }
        public static Result<T> Failure(Error error)
        {
            return new Result<T>(
                false,
                default,
                error);
        }
    }
}
