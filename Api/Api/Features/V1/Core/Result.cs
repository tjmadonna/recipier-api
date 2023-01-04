namespace Api.Features.V1.Core;

public class Result
{
    public bool IsSuccess { get; private set; }

    public string Error { get; private set; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool success, string error)
    {
        if (success && error != string.Empty)
            throw new InvalidOperationException();

        if (!success && error == string.Empty)
            throw new InvalidOperationException();

        IsSuccess = success;
        Error = error;
    }

    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    public static Result<T> Failure<T>(string error)
    {
        return new Result<T>(default!, false, error);
    }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }
}

public class Result<T> : Result
{
    public T Value { get; private set; }

    protected internal Result(T value, bool success, string error) : base(success, error)
    {
        Value = value;
    }
}