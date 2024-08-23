using Novel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novel.DataAccess.Repository.IRepository
{
    public interface ICategoryRepositry : IRepository<Category>
    { 
        void Update(Category category);
      
    }
}
