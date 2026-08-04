namespace WebBanSach.Shared.Dtos.User
{
    // Component dùng: SuaThongTinCaNhan.razor (Customer)
    public record UpdateProfileDto(
        string FullName,
        string? PhoneNumber = null,
        DateTime? DateOfBirth = null,
        string? Address = null,
        string? AvatarBase64 = null,
        string? AvatarFileName = null
    );
}
