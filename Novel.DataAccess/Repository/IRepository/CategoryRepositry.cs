using Novel.DataAccess.Repository.IRepository;
using Novel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Novel.DataAccess.Data;

namespace Novel.DataAccess.Repository
{
    public class CategoryRepositry : Repository<Category>,ICategoryRepositry
    {
        private ApplicationDbContext _db;

        public CategoryRepositry(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            _db.Categories. Update(obj);
        }
    }
}
