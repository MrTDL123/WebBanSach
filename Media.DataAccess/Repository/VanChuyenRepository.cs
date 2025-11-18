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
    public class VanChuyenRepository : Repository<VanChuyen>, IVanChuyenRepository
    {
        private readonly ApplicationDbContext _db;
        public VanChuyenRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
    }
}
