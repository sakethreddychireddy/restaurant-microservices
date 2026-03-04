namespace AuthService.Application.DTOs
{
    public record ApiResponse<T> 
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? ErrorCode { get; init; }
        public T? Data { get; init; }
        public static ApiResponse<T> SuccessResponse(T data, string message = "") => new()
        {
              Success = true,
              Message = message,
              Data = data
        };
        public static ApiResponse<T> FailureResponse(string message, string? errorCode = null) => new()
        {
              Success = false,
              Message = message,
              ErrorCode = errorCode,
              Data = default
        };
    }
}
