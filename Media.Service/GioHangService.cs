using Media.Models;
using Media.Models.ViewModels;
using Media.Utility;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Service
{
    public class GioHangService : IGioHangService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public GioHangService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hàm hỗ trợ mới: Tải giỏ hàng từ DB
        public List<GioHangVM> TaiGioHangTuDb(string userId)
        {
            KhachHang? khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null)
            {
                return new List<GioHangVM>();
            }

            /*
                - phieuGioHang: là đối tượng giỏ hàng, đại diện cho cái phiếu tổng qua, chứa mã 
                khách hàng và tổng tiền, phiếu giỏ hàng này như phần đầu của tờ hóa đơn gồm các
                thông tin khách hàng và tổng số tiền.
             */
            var phieuGioHang = _context.GioHangs
                .Include(gh => gh.ChiTietGioHangs)
                    .ThenInclude(ct => ct.Sach)
                .FirstOrDefault(gh => gh.MaKhachHang == khachHang.MaKhachHang);
            if (phieuGioHang == null || phieuGioHang.ChiTietGioHangs == null)
            {
                return new List<GioHangVM>();
            }

            return phieuGioHang.ChiTietGioHangs.Select(ct => new GioHangVM
            {
                MaSach = ct.MaSach,
                TenSach = ct.Sach.TenSach,
                GiaBan = ct.Sach.GiaBan,
                GiaSauGiam = ct.DonGia,
                AnhBiaChinh = ct.Sach.AnhBiaChinh,
                SoLuong = ct.SoLuong
            }).ToList();
        }

        public void LuuGioHangVaoDb(string userId, List<GioHangVM> gioHang)
        {
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null)
            {
                return;
            }

            var phieuGioHang = _context.GioHangs
                .Include(gh => gh.ChiTietGioHangs)
                .FirstOrDefault(gh => gh.MaKhachHang == khachHang.MaKhachHang);

            if (phieuGioHang == null)
            {
                phieuGioHang = new GioHang
                {
                    MaKhachHang = khachHang.MaKhachHang,
                    TongTien = 0
                };
                _context.GioHangs.Add(phieuGioHang);
                _context.SaveChanges();
            }

            // 1. Xóa tất cả GioHangItem cũ của user này
            if (phieuGioHang.ChiTietGioHangs != null && phieuGioHang.ChiTietGioHangs.Any())
            {
                _context.ChiTietGioHangs.RemoveRange(phieuGioHang.ChiTietGioHangs);
            }

            // 2. Chuyển đổi từ GioHangVM sang model GioHangItem
            var gioHangMoi = gioHang.Select(vm => new ChiTietGioHang
            {
                MaGioHang = phieuGioHang.MaGioHang,
                MaSach = vm.MaSach,
                SoLuong = vm.SoLuong,
                DonGia = vm.GiaSauGiam
            }).ToList();

            if (gioHangMoi.Any())
            {
                _context.ChiTietGioHangs.AddRange(gioHangMoi);
            }

            phieuGioHang.TongTien = gioHangMoi.Sum(ct => ct.ThanhTien);
            _context.SaveChanges();// Lưu thay đổi
        }

        // 3. HÀM GỘP (Move từ KhachHangController)
        public List<GioHangVM> GopGioHangSessionVaoDb(string userId)
        {
            // Lấy từ Session thông qua _session
            var gioHangSession = _session.GetObjectFromJson<List<GioHangVM>>("GioHang") ?? new List<GioHangVM>();

            // Gọi hàm nội bộ
            var gioHangDb = this.TaiGioHangTuDb(userId);

            if (gioHangSession.Any())
            {
                foreach (var sanPhamSession in gioHangSession)
                {
                    var sanPhamDb = gioHangDb.FirstOrDefault(p => p.MaSach == sanPhamSession.MaSach);
                    if (sanPhamDb != null)
                    {
                        sanPhamDb.SoLuong += sanPhamSession.SoLuong;
                    }
                    else
                    {
                        gioHangDb.Add(sanPhamSession);
                    }
                }

                // Gọi hàm nội bộ
                this.LuuGioHangVaoDb(userId, gioHangDb);
            }

            _session.SetObjectAsJson("GioHang", gioHangDb);
            //_session.Remove("GioHangThanhToan");

            return gioHangDb;
        }
    }
}
