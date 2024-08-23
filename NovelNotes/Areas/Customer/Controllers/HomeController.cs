using Microsoft.AspNetCore.Mvc;
using Novel.DataAccess.Repository;
using Novel.DataAccess.Repository.IRepository;
using Novel.Models;
using System.Diagnostics;

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

            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(objProductList);
        }
        public IActionResult Details(int productId)
        {
            Product Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category");
            return View(Product);
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