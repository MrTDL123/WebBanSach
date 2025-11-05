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
        public ChuDe ChuDeHienTai { get; set; }
        public IEnumerable<string>? DanhSachTenNhaXuatBan { get; set; }
        public IEnumerable<string>? DanhSachTenTacGia { get; set; }
        public IPagedList<Sach>? DanhSachSach { get; set; }
        public PriceFilter PriceFilter { get; set; }

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
}
