namespace SmartCart.Backend.DTOs;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public bool IsConflict { get; set; }
    public T? Data { get; set; }

    public static ServiceResult<T> SuccessResult(T data) => new()
    {
        Success = true,
        Data = data,
        Error = null,
        IsConflict = false
    };

    public static ServiceResult<T> FailureResult(string error) => new()
    {
        Success = false,
        Data = default,
        Error = error,
        IsConflict = false
    };

    public static ServiceResult<T> ConflictResult(string error) => new()
    {
        Success = false,
        Data = default,
        Error = error,
        IsConflict = true
    };
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public bool IsConflict { get; set; }

    public static ServiceResult SuccessResult() => new()
    {
        Success = true,
        Error = null,
        IsConflict = false
    };

    public static ServiceResult FailureResult(string error) => new()
    {
        Success = false,
        Error = error,
        IsConflict = false
    };

    public static ServiceResult ConflictResult(string error) => new()
    {
        Success = false,
        Error = error,
        IsConflict = true
    };
}