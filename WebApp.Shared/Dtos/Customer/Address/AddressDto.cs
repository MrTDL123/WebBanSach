using System.Linq;

namespace WebApp.Shared.Dtos.CustomerDtos.Address
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
    )
    {
        public string FullAddress => string.Join(", ", new[] { DetailAddress, Ward, District, Province }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
