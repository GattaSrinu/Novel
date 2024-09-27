using Novel.DataAccess.Repository.IRepository;
using Novel.Models;
using Novel.Models.ViewModels;
using Novel.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using Novel.DataAccess.Repository;
using SendGrid.Helpers.Mail;
using Stripe.Climate;
using System.Linq.Expressions;

namespace NovelNotes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitsOfWork _unitOfWork;

        public UserController(UserManager<IdentityUser> userManager, IUnitsOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {

            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);


            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                //a role was updated
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();


                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            // Prepare the view model
            UserEditVM userEditVM = new UserEditVM()
            {
                ApplicationUser = applicationUser
            };

            return View(userEditVM);
        }

        [HttpPost]
        public IActionResult Edit(UserEditVM userEditVM)
        {
            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userEditVM.ApplicationUser.Id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            // Manual field validation
            if (string.IsNullOrWhiteSpace(userEditVM.ApplicationUser.Name) ||
                string.IsNullOrWhiteSpace(userEditVM.ApplicationUser.StreetAddress) ||
                string.IsNullOrWhiteSpace(userEditVM.ApplicationUser.City) ||
                string.IsNullOrWhiteSpace(userEditVM.ApplicationUser.State) ||
                string.IsNullOrWhiteSpace(userEditVM.ApplicationUser.PostalCode))
            {
                // Add an error message indicating which field is missing or invalid.
                ViewBag.ErrorMessage = "All fields are required.";
                return View(userEditVM);
            }

            // Updating the user details
            applicationUser.Name = userEditVM.ApplicationUser.Name;
            applicationUser.Email = userEditVM.ApplicationUser.Email;
            applicationUser.PhoneNumber = userEditVM.ApplicationUser.PhoneNumber;
            applicationUser.StreetAddress = userEditVM.ApplicationUser.StreetAddress;
            applicationUser.City = userEditVM.ApplicationUser.City;
            applicationUser.State = userEditVM.ApplicationUser.State;
            applicationUser.PostalCode = userEditVM.ApplicationUser.PostalCode;

            // Save updated user info
            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.CompanyId != null)
                {
                    user.Company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == user.CompanyId);
                }

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {

            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
