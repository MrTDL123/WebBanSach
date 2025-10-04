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
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        void Save();
    }
}
