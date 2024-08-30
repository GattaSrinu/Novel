using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novel.DataAccess.Repository.IRepository;
using Novel.DataAccess;

namespace Novel.DataAccess.Repository.IRepository
{
    public interface IUnitsOfWork
    { 
        ICategoryRepositry  Category { get; }
        ICompanyRepository Company { get; }
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        IShoppingCartRepository ShoppingCart { get; }
        ICoverTypeRepository CoverType { get; }
        IProductImageRepository ProductImage { get; }

        void Save();
    }
}
