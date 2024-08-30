using Microsoft.AspNetCore.Mvc;
using Novel.DataAccess.Repository;
using Novel.DataAccess.Repository.IRepository;
using Novel.DataAccess;
using System.Diagnostics;
using Novel.Models;
using Microsoft.AspNetCore.Authorization;
using Novel.Utility;
using System.Security.Claims;

namespace NovelNotes.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitsOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger , IUnitsOfWork  unitOfWork)
        {
            _logger = logger;
             _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");

            return View(objProductList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
            Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
            Count = 1,
            ProductId =productId
            };
            return View(cart);
        }
        
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
               
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}