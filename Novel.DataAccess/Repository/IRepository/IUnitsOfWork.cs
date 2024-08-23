using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novel.DataAccess.Repository.IRepository
{
    public interface IUnitsOfWork
    { 
        ICategoryRepositry  Category { get; }
        ICompanyRepository Company { get; }
        IProductRepositry Product { get; }

        void Save();
    }
}
