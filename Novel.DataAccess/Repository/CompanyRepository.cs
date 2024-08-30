using Novel.DataAccess.Repository.IRepository;
using Novel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novel.DataAccess.Data;

namespace Novel.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Company obj)
        {
            _db.Companies.Update(obj);

            var objFromDB = _db.Companies.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Name = obj.Name;
                objFromDB.StreetAddress = obj.StreetAddress;
                objFromDB.City = obj.City;
                objFromDB.State = obj.State;
                objFromDB.PostalCode = obj.PostalCode;
                objFromDB.PhoneNumber = obj.PhoneNumber;
            }
        }
    }
}
