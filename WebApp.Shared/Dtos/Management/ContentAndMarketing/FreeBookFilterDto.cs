using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public class FreeBookFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string? StockStatus { get; set; }
        // 4. Sắp xếp: "newest" (mới nhất), "most_requested" (thỉnh nhiều nhất), "most_donated" (ủng hộ nhiều nhất)
        public string? SortBy { get; set; } = "newest";
        // 5. Phân trang
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
