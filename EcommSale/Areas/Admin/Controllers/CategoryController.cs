using EcommSale.Data;
using ECommSale.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
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
