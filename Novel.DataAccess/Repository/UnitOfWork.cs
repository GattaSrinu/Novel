using Novel.DataAccess.Repository.IRepository;
using Novel.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novel.DataAccess.Repository.IRepository;

namespace Novel.DataAccess.Repository
{
    public class UnitOfWork : IUnitsOfWork
    {
        private ApplicationDbContext _db;

        public ICategoryRepositry Category { get; private set; }
        public IProductRepositry Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepositry(_db);
            Product = new ProductRepositry(_db);
            Company = new CompanyRepository(_db);

        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
