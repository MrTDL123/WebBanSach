using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
namespace Media.Models.ViewModels
{
    public class SachTheoChuDeVM
    {
        // Breadcurmb URL
        public List<BreadcrumbItem> Breadcrumbs { get; set; }

        // Chủ đề được chọn
        public ChuDe ChuDeCha { get; set; }
        public ChuDe ChuDeSelected { get; set; }

        // Các dữ liệu lọc
        public IEnumerable<NhaXuatBan>? DanhSachNhaXuatBan { get; set; }
        public IEnumerable<TacGia>? DanhSachTacGia { get; set; }
        public PriceFilter PriceFilter { get; set; }

        // ToolBar
        public string SortBy { get; set; }
        public int PageSize { get; set; } = 20;
        public string SearchKeyword { get; set; }


        public IPagedList<Sach>? DanhSachSach { get; set; }
    }

    public class PriceFilter
    {
        public List<PriceRange> PriceRanges { get; set; }
        public List<string> SelectedRanges { get; set; }
    }

    public class PriceRange
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public decimal? MinPrice {  get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
    }
}
