using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Shared.Dtos.User
{
    // DTO xin gửi mã OTP khi đổi Email/SĐT
    public record SendOtpForChangeDto(
        string TargetInfo // Email mới hoặc SĐT mới muốn đổi
    );
}
