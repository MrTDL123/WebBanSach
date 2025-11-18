using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Media.Models.ViewModels
{
<<<<<<<< HEAD:Media.Models/ViewModels/TrangChuVM.cs
    public class TrangChuVM
========
    public class ThongTinMuaHangVM
>>>>>>>> 9db588d3d200938e50878a228249d94651bced20:Media.Models/ViewModels/ThongTinMuaHangVM.cs
    {
        public TrangChuVM()
        {
            DanhSachChuDe = new List<DanhSachChuDeTrangIndex>();
        }
        public List<DanhSachChuDeTrangIndex>? DanhSachChuDe { get; set; }
        public IEnumerable<Sach>? SachBanChay { get; set; }
        public IEnumerable<Sach>? SachGiamGia { get; set; }
        public IEnumerable<Sach> TuSachMienPhi { get; set; }
        public IEnumerable<TacGia> TacGiaNoiTieng { get; set; }
    }

    public class DanhSachChuDeTrangIndex
    {
        public string ChuDeFullPath { get; set; }
        public string TenChuDe { get; set; }
        public string DuongDanHinhAnh {  get; set; }
        public int MaSach { get; set; }
        public int SoLuong { get; set; }
    }
}
