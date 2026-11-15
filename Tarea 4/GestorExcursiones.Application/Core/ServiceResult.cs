namespace GestorExcursiones.Application.Core;

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ServiceResult Ok(string message = "Operación realizada correctamente")
    {
        return new ServiceResult { Success = true, Message = message };
    }

    public static ServiceResult Fail(params string[] errors)
    {
        return new ServiceResult
        {
            Success = false,
            Message = "La operación no pudo completarse",
            Errors = errors.ToList()
        };
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string message = "Operación realizada correctamente")
    {
        return new ServiceResult<T> { Success = true, Message = message, Data = data };
    }

    public new static ServiceResult<T> Fail(params string[] errors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = "La operación no pudo completarse",
            Errors = errors.ToList()
        };
    }
}
