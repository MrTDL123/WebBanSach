using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Models.ViewModels;
using Media.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Service
{
    public class GioHangService : IGioHangService
    {
        private readonly IUnitOfWork _unit;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public GioHangService(IUnitOfWork unit, IHttpContextAccessor httpContextAccessor)
        {
            _unit = unit;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hàm hỗ trợ mới: Tải giỏ hàng từ DB
        public List<GioHangVM> TaiGioHangTuDb(string userId)
        {
            KhachHang khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null)
            {
                return new List<GioHangVM>();
            }

            /*
                - phieuGioHang: là đối tượng giỏ hàng, đại diện cho cái phiếu tổng qua, chứa mã 
                khách hàng và tổng tiền, phiếu giỏ hàng này như phần đầu của tờ hóa đơn gồm các
                thông tin khách hàng và tổng số tiền.
             */
            GioHang phieuGioHang = _unit.GioHangs.Get(gh => gh.MaKhachHang == khachHang.MaKhachHang);
            if (phieuGioHang == null)
            {
                return new List<GioHangVM>();
            }

            int maGioHangThucTe = phieuGioHang.MaGioHang;

            // Lấy tất cả chi tiết giỏ hàng và thông tin Sách đi kèm
            var danhSachSanPhamGioHang = _unit.ChiTietGioHangs.GetRange(
                ct => ct.MaGioHang == maGioHangThucTe,
                includeProperties: "Sach" // Eager loading thông tin Sách
            );

            if (danhSachSanPhamGioHang == null || !danhSachSanPhamGioHang.Any())
            {
                return new List<GioHangVM>();
            }

            // Chuyển đổi từ Model (ChiTietGioHang) sang ViewModel (GioHangVM)
            return danhSachSanPhamGioHang.Select(ct => new GioHangVM
            {
                MaSach = ct.MaSach,
                TenSach = ct.Sach.TenSach,
                GiaBan = ct.Sach.GiaBan,
                GiaSauGiam = ct.DonGia, // Lấy đơn giá đã lưu
                AnhBiaChinh = ct.Sach.AnhBiaChinh,
                SoLuong = ct.SoLuong
            }).ToList();
        }

        public void LuuGioHangVaoDb(string userId, List<GioHangVM> gioHang)
        {
            KhachHang khachHang = _unit.KhachHangs.Get(kh => kh.MaTaiKhoan == userId);
            if (khachHang == null)
            {
                return;
            }
            int maKhachHang = khachHang.MaKhachHang;

            GioHang phieuGioHang = _unit.GioHangs.Get(gh => gh.MaKhachHang == maKhachHang);

            if (phieuGioHang == null)
            {
                phieuGioHang = new GioHang
                {
                    MaKhachHang = maKhachHang,
                    TongTien = 0
                };
                _unit.GioHangs.Add(phieuGioHang);
                _unit.Save();
            }

            int maGioHangThucTe = phieuGioHang.MaGioHang;

            // 1. Xóa tất cả GioHangItem cũ của user này
            var gioHangCu = _unit.ChiTietGioHangs.GetRange(ct => ct.MaGioHang == maGioHangThucTe);
            if (gioHangCu != null && gioHangCu.Any())
            {
                _unit.ChiTietGioHangs.RemoveRange(gioHangCu);
            }


            // 2. Chuyển đổi từ GioHangVM sang model GioHangItem
            var gioHangMoi = gioHang.Select(vm => new ChiTietGioHang
            {
                MaGioHang = maGioHangThucTe,
                MaSach = vm.MaSach,
                SoLuong = vm.SoLuong,
                DonGia = vm.GiaSauGiam
            }).ToList();

            if (gioHangMoi.Any())
            {
                _unit.ChiTietGioHangs.AddRange(gioHangMoi);
            }

            phieuGioHang.TongTien = gioHangMoi.Sum(ct => ct.ThanhTien);
            _unit.Save(); // Lưu thay đổi
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
            _session.Remove("GioHangThanhToan");

            return gioHangDb;
        }
    }
}