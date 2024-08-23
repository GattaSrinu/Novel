
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novel.Models;
using OfficeOpenXml; // Ensure you have this namespace after installing EPPlus
using Novel.DataAccess.Data;
using Novel.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Novel.Utility;



namespace NovelNotes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitsOfWork _categoryrepo;

        public CategoryController(IUnitsOfWork unitsOfWork)
        {
            _categoryrepo = unitsOfWork;
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);

                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                Category category = new Category
                                {
                                    Name = worksheet.Cells[row, 2].Value.ToString(),
                                    DisplayOrder = int.Parse(worksheet.Cells[row, 3].Value.ToString())
                                };

                                _categoryrepo.Category.Add(category);
                            }

                            _categoryrepo.Save();
                        }

                    }
                    ViewBag.Message = "Import successful!";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error occurred: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Error = "Please select a file.";
            }

            return View();
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryrepo.Category.GetAll().ToList();

            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display order cannot exactly match with Name.");
            }

            if (ModelState.IsValid)
            {
                _categoryrepo.Category.Add(obj);
                _categoryrepo.Save();
                TempData["Success"] = "Category created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var CategoryFound = _categoryrepo.Category.Get(u => u.Id == id);

            //Category? CategoryFound = _db.Categories.Find(id);
            //Category? CategoryFound = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? CategoryFound = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (CategoryFound == null)
            {
                return NotFound();
            }
            return View(CategoryFound);

        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            if (ModelState.IsValid)
            {
                _categoryrepo.Category.Update(obj);
                _categoryrepo.Save();
                TempData["Success"] = "Category updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var CategoryFound = _categoryrepo.Category.Get(u => u.Id == id);

            if (CategoryFound == null)
            {
                return NotFound();
            }
            return View(CategoryFound);

        }

        [HttpPost, ActionName("Delete")]

        public IActionResult DeletePost(int? id)
        {
            Category? obj = _categoryrepo.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _categoryrepo.Category.Remove(obj);
            _categoryrepo.Save();
            TempData["Success"] = "Category deleted Successfully";
            return RedirectToAction("Index");


        }
    }
}
