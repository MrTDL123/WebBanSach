using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    [PrimaryKey(nameof(MaKhachHang), nameof(MaDanhGia))]
    public class LuotThichDanhGiaSanPham
    {
        public int MaKhachHang { get; set; }
        [ForeignKey("MaKhachHang")]
        [ValidateNever]
        public KhachHang KhachHang { get; set; }

        public int MaDanhGia { get; set; }
        [ForeignKey("MaDanhGia")]
        [ValidateNever]
        public DanhGiaSanPham DanhGiaSanPham { get; set; }
    }
}