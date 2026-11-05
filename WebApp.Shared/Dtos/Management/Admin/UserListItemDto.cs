namespace WebApp.Shared.Dtos.Management.Admin
{
    // Component dùng: QuanLyNguoiDung.razor (Admin)
    public record UserListItemDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public int TotalOrders { get; set; }

        public UserListItemDto() { }
    }
}
