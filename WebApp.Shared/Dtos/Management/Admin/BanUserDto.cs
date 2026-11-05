using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.Admin
{
    public record BanUserDto
    {
        public string? UserId { get; set; }
        public string? Reason { get; set; }
    }
}
