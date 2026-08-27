namespace WebApp.Shared.Dtos.Management.CustomerSupport
{
    // Component dùng: DuyetThinSach.razor (Admin - CSKH)
    public record ReviewBookRequestDto(
        int RequestId,
        bool IsApproved,
        string? RejectedReason = null,
        string? EmployeeNote = null
    );
}
