namespace WarehouseFlow.Api.Contracts
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Message { get; init; }
        public IReadOnlyCollection<string>? Errors { get; init; }
        public int StatusCode { get; init; }

        public static ApiResponse<T> SuccessResult(
            T data,
            string? message = null,
            int statusCode = StatusCodes.Status200OK
        ) =>
            new()
            {
                Data = data,
                StatusCode = statusCode,
                Message = message,
                Success = true,
                Errors = null,
            };

        public static ApiResponse<T> CreatedResult(T data, string? message = null) =>
            SuccessResult(data, message, StatusCodes.Status201Created);

        public static ApiResponse<T> FailureResult(
            string message,
            IEnumerable<string>? errors = null,
            int statusCode = StatusCodes.Status500InternalServerError
        ) =>
            new()
            {
                Success = false,
                Message = message,
                Errors = errors?.ToArray() ?? Array.Empty<string>(),
                StatusCode = statusCode,
            };
    }
}
