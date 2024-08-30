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
    public class ProductRepositry : Repository<Product>, IProductRepository
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
                objFromDB.Title = obj.Title;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Price = obj.Price;
                objFromDB.Price50 = obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.Description = obj.Description;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.Author = obj.Author;
                objFromDB.ProductImages = obj.ProductImages;

                //if (obj.ImageUrl != null)
                //{
                //    objFromDB.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
    