using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class DonHangRepository : Repository<DonHang>, IDonHangRepository
    {
        private readonly ApplicationDbContext _db;
        public DonHangRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
    }
}