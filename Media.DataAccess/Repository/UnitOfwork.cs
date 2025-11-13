using Media.DataAccess.Repository.IRepository;
using Meida.DataAccess.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository
{
    public class UnitOfwork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        //Add Table
        public IChuDeRepository ChuDes { get; private set; }
        public ISachRepository Saches { get; private set; }

        public ITacGiaRepository TacGias { get; private set; }

        public INhaXuatBanRepository NhaXuatBans { get; private set; }

        public UnitOfwork(ApplicationDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
            ChuDes = new ChuDeRepository(_db);
            Saches = new SachRepository(_db, _cache);
            TacGias = new TacGiaRepository(_db);
            NhaXuatBans = new NhaXuatBanRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
