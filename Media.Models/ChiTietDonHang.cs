using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTietDonHang { get; set; }
        [Required]
        [DisplayName("Số lượng")]
        public int SoLuong { get; set; }
        [Required]
        [DisplayName("Đơn giá")]
        public decimal DonGia { get; set; }


        //NAVIGATION PROPERTIES
        public int MaSach { get; set; }
        public int MaDonHang { get; set; }  

        [ForeignKey("MaSach")]
        public Sach Sach { get; set; }
        [ForeignKey("MaDonHang")]
        public DonHang DonHang { get; set; }
    }
}
