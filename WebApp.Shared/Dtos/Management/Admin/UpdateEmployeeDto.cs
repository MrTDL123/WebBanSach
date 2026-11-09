using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.Admin
{
    public record UpdateEmployeeDto
    {
        public string? UserId { get; set; }
        public string? Role { get; set; }

        public UpdateEmployeeDto() { }
    }
}
