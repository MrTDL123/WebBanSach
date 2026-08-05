namespace WebApp.Shared.Dtos.CustomerDtos.FreeBook
{
    // Component dùng: ThinSachForm.razor (Customer)
    public record CreateBookRequestDto(
        int FreeBookId,
        int QuantityRequested,
        string Reason,
        string ReceiverName,
        string ReceiverPhone,
        string ShippingAddress,
        string? IntendedUse = null
    );
}
