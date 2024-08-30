
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novel.Models;
using OfficeOpenXml; // Ensure you have this namespace after installing EPPlus
using Novel.DataAccess.Data;
using Novel.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Novel.Models.ViewModels;
using Microsoft.AspNetCore.Components.RenderTree;
using Novel.DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Novel.Utility;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Security.Cryptography.X509Certificates;



namespace NovelNotes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitsOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitsOfWork unitsOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitsOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);

                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null)
                {

                    foreach (var file in files)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);
                        using (var fileStream = new FileStream(Path.Combine(finalPath, filename), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + filename,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }

                TempData["Success"] = "Category created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ToString()
                });
                return View(productVM);
            }
        }

        public IActionResult DeleteImage(int ImageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == ImageId);
            int productId = imageToBeDeleted.ProductId;

            if (imageToBeDeleted == null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                    _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                    _unitOfWork.Save();

                    TempData["success"] = "Deleted successfully";
              
            }
            return RedirectToAction(nameof(Upsert), new { id = productId });
        }

            #region API CALLS
            [HttpGet]

            public IActionResult GetAll()
            {
                List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

                return Json(new { data = objProductList });
            }

            [HttpDelete]
            public IActionResult Delete(int? id)
            {
                var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
                if (productToBeDeleted == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }

                string productPath = @"images\products\product-" + id;
                string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

                if (Directory.Exists(finalPath))
                {
                    string[] filepaths = Directory.GetFiles(finalPath);
                    foreach (string filepath in filepaths)
                    {
                        System.IO.File.Delete(filepath);
                    }
                    Directory.Delete(finalPath);
                }

                _unitOfWork.Product.Remove(productToBeDeleted);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Delete Successful" });


            }
            #endregion
    }
}

