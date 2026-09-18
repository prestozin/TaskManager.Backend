namespace TaskManager.Application.DTOs;

public class ResultResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }


    private ResultResponse(bool isSuccess, string? message, T? data)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static ResultResponse<T> Success(T? data, string? message = "Operation completed successfully")
    {
        return new ResultResponse<T>(true, message, data);
    }

    public static ResultResponse<T> Success(string? message = "Operation completed successfully")
    {
        return new ResultResponse<T>(true, message, default);
    }

    public static ResultResponse<T> Failure(T? data, string? message = "Operation failed")
    {
        return new ResultResponse<T>(false, message, data);
    }

    public static ResultResponse<T> Failure(string? message = "Operation failed")
    {
        return new ResultResponse<T>(false, message, default);
    }

    public static ResultResponse<T> ValidationFailure(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
    {
        return new ResultResponse<T>(false, "Erro de validação", default)
        {
            Errors = errors.Select(x => x.ErrorMessage).ToList()
        };
    }
}
