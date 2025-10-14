using System;
using System.Collections.Generic;
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
        public int ID { get; set; }
        [Required]
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        //NAVIGATION PROPERTIES
        public int SachId { get; set; }
        public int DonHangId { get; set; }

        [ForeignKey("SachId")]
        public Sach Sach { get; set; }
        [ForeignKey("DonHangId")]
        public DonHang DonHang { get; set; }
    }
}
