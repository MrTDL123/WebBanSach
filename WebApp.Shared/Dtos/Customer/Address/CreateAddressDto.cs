namespace WebBanSach.Shared.Dtos.CustomerDtos.Address
{
    // Component dùng: ThemDiaChi.razor (Customer - Sổ địa chỉ)
    // Khách hàng thêm địa chỉ nhận hàng mới vào sổ địa chỉ
    public record CreateAddressDto(
        string ReceiverName,
        string ReceiverPhone,
        string Province,
        string District,
        string Ward,
        string DetailAddress,
        bool SetAsDefault = false
    );
}
