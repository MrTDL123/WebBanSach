namespace WebBanSach.Shared.Dtos.AdminClient.CustomerSupport
{
    // Component dùng: DuyetThinSach.razor (Admin - CSKH)
    public record ReviewBookRequestDto(
        int RequestId,
        bool IsApproved,
        string? RejectedReason = null,
        string? EmployeeNote = null
    );
}
