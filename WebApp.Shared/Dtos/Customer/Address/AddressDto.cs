namespace WebBanSach.Shared.Dtos.CustomerDtos.Address
{
    // Component dùng: SoNguonTinh.razor, Checkout.razor (Customer)
    // Trả về danh sách địa chỉ đã lưu của khách hàng
    public record AddressDto(
        int AddressId,
        string ReceiverName,
        string ReceiverPhone,
        string Province,
        string District,
        string Ward,
        string DetailAddress,
        bool IsDefault
    );
}
