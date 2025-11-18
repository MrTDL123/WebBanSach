using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class DanhGiaSanPhamRepository : Repository<DanhGiaSanPham>, IDanhGiaSanPhamRepository
    {
        private readonly ApplicationDbContext _db;
        public DanhGiaSanPhamRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}