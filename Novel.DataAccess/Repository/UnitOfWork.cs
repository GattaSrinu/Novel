using Novel.DataAccess.Repository.IRepository;
using Novel.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novel.Models;

namespace Novel.DataAccess.Repository
{
    public class UnitOfWork : IUnitsOfWork
    {
        private ApplicationDbContext _db;

        public ICategoryRepositry Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public  IProductImageRepository ProductImage {  get; private set; }   
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepositry(_db);
            Product = new ProductRepositry(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            ProductImage = new ProductImageRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
