
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



namespace NovelNotes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitsOfWork _prodcutrepo;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitsOfWork unitsOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _prodcutrepo = unitsOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objCategoryList = _prodcutrepo.Product.GetAll(includeProperties:"Category").ToList();

            return View(objCategoryList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _prodcutrepo.Category.GetAll().Select(u => new SelectListItem
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
                productVM.Product = _prodcutrepo.Product.Get(u =>u.Id == id);

                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? image)
        {

            if (ModelState.IsValid)
            { 
                string wwRootpath = _webHostEnvironment.WebRootPath;

                if(image != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string productpath =Path.Combine(wwRootpath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old Image  , whenever update
                        var oldImagePath = Path.Combine(wwRootpath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using( var filestream = new FileStream(Path.Combine(productpath,filename), FileMode.Create))
                    {
                        image.CopyTo(filestream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                }

                if(productVM.Product.Id==0)
                {
                    _prodcutrepo.Product.Add(productVM.Product);
                }
                else
                {
                    _prodcutrepo.Product.Update(productVM.Product);
                }
             
                _prodcutrepo.Save();
                TempData["Success"] = "Product created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _prodcutrepo.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(productVM);
            }
        }

        //[HttpGet]
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var ProductFound = _prodcutrepo.Product.Get(u => u.Id == id);

        //    //Product? ProductFound = _db..Find(id);
        //    //Product? ProductFound = _db.Products.FirstOrDefault(u => u.Id == id);
        //    //Product? ProductFound = _db.Products.Where(u => u.Id == id).FirstOrDefault();

        //    if (ProductFound == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductFound);

        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _prodcutrepo.Product.Update(obj);
        //        _prodcutrepo.Save();
        //        TempData["Success"] = "Product updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var ProductFound = _prodcutrepo.Product.Get(u => u.Id == id);

        //    if (ProductFound == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductFound);

        //}

        //[HttpPost, ActionName("Delete")]

        //public IActionResult DeletePost(int? id)
        //{
        //    var obj = _prodcutrepo.Product.Get(u => u.Id == id);

        //    if (obj == null)
        //    {
        //        return Json(new { success = false, message = "Product not found." });
        //    }
        //    _prodcutrepo.Product.Remove(obj);
        //    _prodcutrepo.Save();

        //    return Json(new { success = true, message = "Product deleted successfully." });
        //}


        #region API CALLS
        [HttpGet]

        public IActionResult GetAll()
        {
        List<Product> objCategoryList = _prodcutrepo.Product.GetAll(includeProperties: "Category").ToList();

        return Json(new { data = objCategoryList });
        }
    
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _prodcutrepo.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            _prodcutrepo.Product.Remove(productToBeDeleted);
            _prodcutrepo.Save();

            return Json(new { success = true, message = "Delete Successful" });
  

        }
        #endregion
    }
}
