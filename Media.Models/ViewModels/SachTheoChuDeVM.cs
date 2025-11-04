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
        

    }
}
