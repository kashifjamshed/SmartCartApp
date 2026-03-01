namespace SmartCart.Backend.DTOs;

/// <summary>
/// Generic service result wrapper for operations that return data.
/// </summary>
/// <typeparam name="T">The type of data being returned on success.</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Gets or sets whether the operation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets whether the operation failed due to a conflict (e.g., insufficient stock).
    /// </summary>
    public bool IsConflict { get; set; }

    /// <summary>
    /// Gets or sets the data returned on success.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    public static ServiceResult<T> SuccessResult(T data) => new()
    {
        Success = true,
        Data = data,
        Error = null,
        IsConflict = false
    };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static ServiceResult<T> FailureResult(string error) => new()
    {
        Success = false,
        Data = default,
        Error = error,
        IsConflict = false
    };

    /// <summary>
    /// Creates a conflict result (e.g., insufficient stock).
    /// </summary>
    public static ServiceResult<T> ConflictResult(string error) => new()
    {
        Success = false,
        Data = default,
        Error = error,
        IsConflict = true
    };
}

/// <summary>
/// Service result wrapper for operations that don't return data.
/// </summary>
public class ServiceResult
{
    /// <summary>
    /// Gets or sets whether the operation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets whether the operation failed due to a conflict (e.g., insufficient stock).
    /// </summary>
    public bool IsConflict { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static ServiceResult SuccessResult() => new()
    {
        Success = true,
        Error = null,
        IsConflict = false
    };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static ServiceResult FailureResult(string error) => new()
    {
        Success = false,
        Error = error,
        IsConflict = false
    };

    /// <summary>
    /// Creates a conflict result (e.g., insufficient stock).
    /// </summary>
    public static ServiceResult ConflictResult(string error) => new()
    {
        Success = false,
        Error = error,
        IsConflict = true
    };
}
