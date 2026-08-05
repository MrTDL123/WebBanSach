namespace WebApp.Shared.Dtos.CustomerDtos.Review
{
    // Component dùng: FormDanhGia.razor (Customer - chỉ hiển thị sau khi đơn hàng Completed)
    // Điều kiện: IsVerifiedPurchase - khách phải đã mua và nhận hàng thành công mới được đánh giá
    public record CreateReviewDto(
        int ProductId,
        int OrderId,      // Dùng để hệ thống tự động xác nhận IsVerifiedPurchase
        int Rating,       // Bắt buộc: 1-5 sao
        string? Comment   // Tùy chọn: nội dung nhận xét bằng chữ
    );
}
