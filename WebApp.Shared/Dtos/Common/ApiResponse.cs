namespace WebApp.Shared.Dtos.Common
{
    // Wrapper chuẩn phản hồi từ Web API trả về cho 2 Blazor Client
    public record ApiResponse<T>(
        bool Success,
        string Message,
        T? Data = default,
        List<string>? Errors = null
    )
    {
        public static ApiResponse<T> SuccessResult(T data, string message = "Thành công")
            => new(true, message, data);

        public static ApiResponse<T> FailureResult(string message, List<string>? errors = null)
            => new(false, message, default, errors);
    }

    // Dùng cho dữ liệu danh sách có phân trang (Pagination)
    public record PagedResult<T>(
        List<T> Items,
        int TotalCount,
        int Page,
        int PageSize
    )
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 1));
    }
}