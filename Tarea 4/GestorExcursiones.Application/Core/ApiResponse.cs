namespace GestorExcursiones.Application.Core;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> FromResult(ServiceResult<T> result)
    {
        return new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data,
            Errors = result.Errors
        };
    }

    public static ApiResponse<T> FromResult(ServiceResult result)
    {
        return new ApiResponse<T>
        {
            Success = result.Success,
            Message = result.Message,
            Errors = result.Errors
        };
    }
}
