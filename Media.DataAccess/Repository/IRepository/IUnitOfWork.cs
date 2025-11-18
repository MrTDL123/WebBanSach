using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        //Add Table
        IChuDeRepository ChuDes { get; }
        ISachRepository Saches { get; }
        ITacGiaRepository TacGias { get; }
        INhaXuatBanRepository NhaXuatBans { get; }
        IKhachHangRepository KhachHangs { get; }
        IGioHangRepository GioHangs { get; }
        IChiTietGioHangRepository ChiTietGioHangs { get; }
        IDiaChiNhanHangRepository DiaChiNhanHangs { get; }
        IDonHangRepository DonHangs { get; }
        IHoaDonRepository HoaDons { get; }
        IVanChuyenRepository VanChuyens { get; }
        IDanhGiaSanPhamRepository DanhGiaSanPhams { get; }
        ILuotThichDanhGiaSanPhamRepository LuotThichDanhGiaSanPhams { get; }
        void Save();
        Task SaveAsync();
    }
}
