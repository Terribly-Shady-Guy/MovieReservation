using System.Diagnostics.CodeAnalysis;

namespace ApplicationLogic.ViewModels
{
    public class Result
    {
        public virtual bool Successful { get; }

        [MemberNotNullWhen(true, nameof(Error))]
        public bool Failure => !Successful;
        public string? Error { get; }

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

        public static Result Success()
        {
            return new Result();
        }

        public static Result Fail(string error)
        {
            return new Result(error);
        }
    }

    public sealed class Result<TValue> : Result
    {
        // Workaround to handle nullability warnings
        [MemberNotNullWhen(true, nameof(Value))]
        public override bool Successful => base.Successful;

        public TValue? Value {  get; }

        private Result(TValue value) : base()
        {
            Value = value;
        }

        private Result(string error) : base(error)
        { }

        public static Result<TValue> Success(TValue value)
        {
            return new Result<TValue>(value);
        }

        public static new Result<TValue> Fail(string error)
        {
            return new Result<TValue>(error);
        }
    }
}
