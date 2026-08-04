namespace WebBanSach.Shared.Dtos.CustomerDtos.Review
{
    // Component dùng: DanhGiaSanPham.razor (Customer - phần hiển thị đánh giá)
    // Đây là dữ liệu READ-ONLY, API trả về để hiển thị trên trang sách
    public record ProductReviewDto(
        int ReviewId,
        string UserId,
        string? ReviewerName,     // Họ tên rút gọn để bảo vệ privacy: "Nguyễn V. A."
        string? ReviewerAvatarUrl,
        int Rating,               // 1-5 sao
        string? Comment,
        DateTime CreatedAt,
        bool IsVerifiedPurchase,  // True: khách này đã mua và nhận sách thành công
        int TotalLikes,
        bool CurrentUserLiked     // True: user đang đăng nhập đã like review này rồi
    );
}
