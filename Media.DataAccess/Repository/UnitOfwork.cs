using Media.DataAccess.Repository.IRepository;
using Meida.DataAccess.Data;
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
        //Add Table
        public IChuDeRepository ChuDes { get; private set; }
        public ISachRepository Saches { get; private set; }
        public UnitOfwork(ApplicationDbContext db)
        {
            _db = db;
            ChuDes = new ChuDeRepository(_db);
            Saches = new SachRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
