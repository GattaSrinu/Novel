using Novel.DataAccess.Repository.IRepository;
using Novel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Novel.DataAccess.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Novel.DataAccess.Repository
{
    public class ProductRepositry : Repository<Product>,IProductRepositry
    {
        private ApplicationDbContext _db;

        public ProductRepositry(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            _db.Products. Update(obj);

            var objFromDB = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Title = objFromDB.Title;
                objFromDB.ISBN = objFromDB.ISBN;
                objFromDB.Price = objFromDB.Price;
                objFromDB.Price50 = objFromDB.Price50;
                objFromDB.Price100 = objFromDB.Price100;
                objFromDB.Description = objFromDB.Description;
                objFromDB.CategoryId = objFromDB.CategoryId;
                objFromDB.Author = objFromDB.Author;

                if(obj.ImageUrl != null)
                {
                    objFromDB.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
    