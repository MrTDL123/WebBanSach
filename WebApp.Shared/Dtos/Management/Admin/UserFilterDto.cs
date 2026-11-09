using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.Admin
{
    public class UserFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
