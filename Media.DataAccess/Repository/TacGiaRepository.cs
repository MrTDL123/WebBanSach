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
    public class TacGiaRepository : Repository<TacGia>, ITacGiaRepository
    {
        private readonly ApplicationDbContext _db;
        public TacGiaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(TacGia obj)
        {
            var objFromDb = _db.TacGias.FirstOrDefault(tg => tg.MaTacGia == obj.MaTacGia);

            if (objFromDb != null)
            {
                objFromDb.TenTG = obj.TenTG;
            }
        }
    }
}
