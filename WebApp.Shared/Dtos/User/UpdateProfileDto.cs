using System;

namespace WebApp.Shared.Dtos.User
{
    // Component dùng: SuaThongTinCaNhan.razor (Customer), Profile.razor (Admin) và Android Mobile
    public record UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = null;
        public DateTime? DateOfBirth { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? AvatarUrl { get; set; } = null;
        public string? AvatarBase64 { get; set; } = null;
        public string? AvatarFileName { get; set; } = null;

        public UpdateProfileDto() { }

        public UpdateProfileDto(
            string fullName,
            DateTime? dateOfBirth = null,
            string? address = null,
            string? avatarBase64 = null,
            string? avatarFileName = null,
            string? phoneNumber = null,
            string? gender = null,
            string? avatarUrl = null)
        {
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Address = address;
            AvatarBase64 = avatarBase64;
            AvatarFileName = avatarFileName;
            PhoneNumber = phoneNumber;
            AvatarUrl = avatarUrl;
        }
    }
}
