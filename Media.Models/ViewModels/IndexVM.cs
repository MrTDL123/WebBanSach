using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Media.Models.ViewModels
{
    public class IndexVM
    {
        public IndexVM()
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
    }
}
