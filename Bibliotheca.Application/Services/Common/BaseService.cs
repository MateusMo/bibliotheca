using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Services;

public abstract class BaseService
{
    protected static ResponseDto<T> Success<T>(
        T data,
        string message = "Success",
        int statusCode = 200)
    {
        return new ResponseDto<T>
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    protected static ResponseDto<T> Failure<T>(
        string message,
        int statusCode = 400)
    {
        return new ResponseDto<T>
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Message = message
        };
    }
}