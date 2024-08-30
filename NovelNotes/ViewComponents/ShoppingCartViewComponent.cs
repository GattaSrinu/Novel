using Novel.DataAccess.Repository.IRepository;
using Novel.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents {
    public class ShoppingCartViewComponent : ViewComponent {

        private readonly IUnitsOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitsOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null) {

                if (HttpContext.Session.GetInt32(SD.SessionCart) == null) {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
