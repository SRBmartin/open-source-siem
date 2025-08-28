namespace Siem.Platform.Shared.Application.Abstractions.Common.Http;

public sealed record Error(string Code, string Message);

public class Result
{
    public bool IsSuccess { get; }
    public List<Error> Errors { get; }

    protected Result(bool isSuccess, List<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new();
    }

    public static Result Success() => new(true);
    public static Result Failure(params Error[] errors) => new(false, errors.ToList());
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors.ToList());
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, List<Error>? errors = null) : base(isSuccess, errors)
        => Value = value;

    public static Result<T> Success(T value) => new(true, value);
    public static new Result<T> Failure(params Error[] errors) => new(false, default, errors.ToList());
    public static new Result<T> Failure(IEnumerable<Error> errors) => new(false, default, errors.ToList());
}
