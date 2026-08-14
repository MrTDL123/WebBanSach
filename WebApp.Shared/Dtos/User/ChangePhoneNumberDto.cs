using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.User
{
    // DTO thực hiện Đổi Số điện thoại
    public record ChangePhoneNumberDto(
        string NewPhoneNumber,
        string OtpCode
    );
}
