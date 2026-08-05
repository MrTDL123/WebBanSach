namespace WebApp.Shared.Dtos.User
{
    // Component dùng: ThongTinCaNhan.razor (Customer)
    public record UserProfileDto(
        string UserId,
        string FullName,
        string? Email,
        string? PhoneNumber,
        DateTime? DateOfBirth,
        string? Address,
        string? AvatarUrl,
        int TotalOrders,
        int TotalWishlistItems
    );
}
