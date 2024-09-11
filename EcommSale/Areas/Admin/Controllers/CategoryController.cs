using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db;

        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }
        
        public IActionResult Index()
        {
            return View(db.Category.ToList());
        }

        //Create get action method
        public ActionResult Create()
        {
            return View();
        }

        //Create post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var searchCategory = db.Category.FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (searchCategory != null) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
                {
                    ViewBag.ExistErrorCate = "This category already exists";
                    return View(category);
                }

                db.Category.Add(category);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category); 
        }

        //Edit method get
        public ActionResult Edit(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }
            var category = db.Category.Find(id);
            if (category == null) {
                return NotFound();
            }
            return View(category);
        }

        //Edit method post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var searchCategory = db.Category.AsNoTracking().FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (searchCategory != null && searchCategory.CategoryID != category.CategoryID) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
                {
                    ViewBag.ExistErrorCate = "This category already exists";
                    return View(category);
                }

                db.Update(category);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //Delete method get
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = db.Category.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            if (TempData.ContainsKey("deleteError"))
            {
                // Pass the delete error message to the view using ViewBag
                ViewBag.DeleteError = TempData["deleteError"];
            }

            return View(category);
        }

        //Delete method post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id, Category category)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (id != category.CategoryID) {
                return NotFound();
            }
            var cate = db.Category.Find(id);
            if (cate == null) {
                return NotFound();
            }

            var productsInCategory = await db.Product.AnyAsync(p => p.CategoryID == id);
            if (productsInCategory)
            {
                // If there are products in this category, display a message
                TempData["deleteError"] = "Cannot delete category. There are products associated with this category.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (ModelState.IsValid)
            {
                db.Remove(cate);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

    }
}
