using Novel.DataAccess.Data;
using Novel.DataAccess.Repository.IRepository;
using Novel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Novel.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository 
        {
        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        

        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
