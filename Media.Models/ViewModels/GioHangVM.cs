using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models.ViewModels
{
    public class GioHangVM
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string AnhBiaChinh { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaSauGiam { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => GiaSauGiam * SoLuong;
    }
}
