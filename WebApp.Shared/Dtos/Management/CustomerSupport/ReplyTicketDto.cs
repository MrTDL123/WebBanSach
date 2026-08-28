using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Shared.Dtos.Management.CustomerSupport
{
    // Nhân viên CSKH hoặc Khách hàng gửi thêm tin nhắn vào ticket
    public record ReplyTicketDto(
        int TicketId,
        string Content
    );
}
