using System.Diagnostics.CodeAnalysis;

namespace ApplicationLogic.ViewModels
{
    public interface IAppResult
    {
        public bool Successful { get; }
        public bool Failure { get; }
        public string? Error { get; }
    }

    public class Result : IAppResult
    {
        protected Result()
        {
            Successful = true;
            Error = null;
        }

        protected Result(string error)
        {
            Successful = false;
            Error = error;
        }

        [MemberNotNullWhen(false, nameof(Error))]
        public bool Successful { get; }

        [MemberNotNullWhen(true, nameof(Error))]
        public bool Failure => !Successful;
        public string? Error { get; }

        public static Result Success()
        {
            return new Result();
        }

        public static Result Fail(string error)
        {
            return new Result(error);
        }
    }

    public sealed class Result<TValue> : IAppResult
    {
        private Result(TValue value)
        {
            Value = value;
        }

        private Result(string error)
        { 
            Error = error;
        }

        public TValue? Value { get; }

        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Error))]
        public bool Successful => Value != null;
        [MemberNotNullWhen(true, nameof(Error))]
        [MemberNotNullWhen(false, nameof(Value))]
        public bool Failure => !Successful;
        public string? Error { get; }

        public static Result<TValue> Success(TValue value)
        {
            return new Result<TValue>(value);
        }

        public static Result<TValue> Fail(string error)
        {
            return new Result<TValue>(error);
        }
    }
}
