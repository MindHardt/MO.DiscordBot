namespace Application;

public static class Results
{
    /// <summary>
    /// Represents an action result that can either succeed or fail.
    /// </summary>
    public class Result
    {
        protected const string NoExceptionMessage = $"Cannot retrieve {nameof(Exception)} of successful result";
        protected readonly Exception? InnerException;

        protected Result()
        {
        }

        protected Result(Exception? exception)
        {
            InnerException = exception;
        }

        /// <summary>
        /// The exception that occured, if any.
        /// </summary>
        public Exception Exception => InnerException ?? throw new InvalidOperationException(NoExceptionMessage);

        /// <summary>
        /// Indicates that this <see cref="ValueResult{T}"/> completed successfully.
        /// </summary>
        public bool Success => InnerException is null;

        public static Result Successful { get; } = new(exception: null);
        public static Result Failed(Exception exception) => new(exception);
    }

    /// <summary>
    /// Represents an action result that can either retrieve value or fail.
    /// </summary>
    /// <typeparam name="T">The contained value.</typeparam>
    public class ValueResult<T> : Result
    {
        protected const string NoValueMessage = $"Cannot retrieve {nameof(Value)} of a failed result";
        protected readonly T? InnerValue;

        /// <summary>
        /// The contained value, if any.
        /// </summary>
        public T Value => InnerValue ?? throw new InvalidOperationException(NoValueMessage);

        private ValueResult(Exception? exception) : base(exception)
        { }

        private ValueResult(T value)
        {
            InnerValue = value;
        }

        public new static ValueResult<T> Successful(T value) => new(value);
        public new static ValueResult<T> Failed(Exception exception) => new(exception);
    }

    /// <summary>
    /// Wraps this <see cref="Action"/> as <see cref="Result"/>.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Result AsResult(this Action action)
    {
        try
        {
            action();
            return Result.Successful;
        }
        catch (Exception e)
        {
            return Result.Failed(e);
        }
    }

    /// <summary>
    /// Wraps this <see cref="Task"/> as <see cref="Result"/>.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async Task<Result> AsResult(this Task task)
    {
        try
        {
            await task;
            return Result.Successful;
        }
        catch (Exception e)
        {
            return Result.Failed(e);
        }
    }

    /// <summary>
    /// Wraps this <see cref="Func{TResult}"/> as <see cref="ValueResult{T}"/>.
    /// </summary>
    /// <param name="func"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ValueResult<T> AsResult<T>(this Func<T> func)
    {
        try
        {
            return ValueResult<T>.Successful(func());
        }
        catch (Exception e)
        {
            return ValueResult<T>.Failed(e);
        }
    }

    /// <summary>
    /// Wraps this <see cref="Task{TResult}"/> as <see cref="ValueResult{T}"/>.
    /// </summary>
    /// <param name="task"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<ValueResult<T>> AsResult<T>(this Task<T> task)
    {
        try
        {
            return ValueResult<T>.Successful(await task);
        }
        catch (Exception e)
        {
            return ValueResult<T>.Failed(e);
        }
    }
}