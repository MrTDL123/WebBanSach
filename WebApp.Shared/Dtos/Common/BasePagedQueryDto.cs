namespace WebBanSach.Shared.Dtos.Common
{
    // DTO cơ sở dùng làm Query Parameter cho tất cả các API lấy danh sách phân trang (Admin & Customer)
    public record BasePagedQueryDto(
        string? SearchTerm = null,
        int PageIndex = 1,
        int PageSize = 10,
        string? SortBy = null,
        bool IsDescending = false
    );
}
