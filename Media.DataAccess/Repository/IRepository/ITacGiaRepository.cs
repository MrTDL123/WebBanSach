using Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataAccess.Repository.IRepository
{
    public interface ITacGiaRepository : IRepository<TacGia>
    {
        void Update(TacGia obj);
    }
}
