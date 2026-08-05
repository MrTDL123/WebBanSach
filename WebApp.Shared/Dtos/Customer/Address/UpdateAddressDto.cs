namespace WebApp.Shared.Dtos.CustomerDtos.Address
{
    // Component dùng: SuaDiaChi.razor (Customer - Sổ địa chỉ)
    // Khách hàng chỉnh sửa một địa chỉ đã lưu
    public record UpdateAddressDto(
        int AddressId,
        string ReceiverName,
        string ReceiverPhone,
        string Province,
        string District,
        string Ward,
        string DetailAddress,
        bool SetAsDefault = false
    );
}
