namespace WebApp.Shared.Dtos.CustomerDtos.FreeBook
{
    // Component dùng: TangSachForm.razor (Customer)
    public record CreateBookDonationDto(
        string DonorName,
        string DonorPhone,
        string PickupAddress,
        List<DonationBookItemDto> Books,
        string? DonorEmail = null,
        string? DonorMessage = null
    );
}
