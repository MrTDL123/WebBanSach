using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Common
{
    public record PagedResultDto<T>
    {
        public List<T> Items { get; init; } = new();
        public int TotalCount { get; init; }
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 10));
        public PagedResultDto() { }
    }
}
