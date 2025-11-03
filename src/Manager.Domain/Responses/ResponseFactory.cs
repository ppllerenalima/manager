namespace Manager.Domain.Responses
{
    public static class ResponseFactory
    {
        public static BaseResponse Success(string message = "Operación realizada con éxito", int statusCode = 200)
        {
            return new BaseResponse
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static BaseResponseGeneric<T> Success<T>(T data, string message = "Operación realizada con éxito", int statusCode = 200)
        {
            return new BaseResponseGeneric<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static BaseResponse Error(string message, string? errorCode = null, int statusCode = 500)
        {
            return new BaseResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };
        }

        public static BaseResponseGeneric<T> Error<T>(string message, string? errorCode = null, int statusCode = 500)
        {
            return new BaseResponseGeneric<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };
        }
    }
}
