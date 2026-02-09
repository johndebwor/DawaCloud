namespace DawaCloud.Web.Shared;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }

    protected Result() { }

    public static Result Ok() => new() { IsSuccess = true };
    public static Result Fail(string error) => new() { IsSuccess = false, Error = error };
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    private Result() : base() { }

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public new static Result<T> Fail(string error) => new() { IsSuccess = false, Error = error };
}
