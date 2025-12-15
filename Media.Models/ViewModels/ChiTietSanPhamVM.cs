using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models.ViewModels
{
    public class ChiTietSanPhamVM
    {
        public Sach Sach { get; set; }
        public List<DanhGiaSanPham> DanhSachDanhGiaSanPham { get; set; }
        public double DiemDanhGiaSanPhamTrungBinh { get; set; }
        public int TongSoDanhGiaSanPham { get; set; }
        public Dictionary<int, int> PhanTramTheoSoSao { get; set; }
        public int SoLuongSachBan {  get; set; }

    }
}