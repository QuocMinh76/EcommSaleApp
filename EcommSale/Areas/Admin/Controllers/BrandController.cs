using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private ApplicationDbContext db;

        public BrandController(ApplicationDbContext db) { 
            this.db = db;
        }
        public IActionResult Index()
        {
            return View(db.Brand.ToList());
        }

        
        //Create get action method
        public ActionResult Create()
        {
            return View();
        }

        //Create post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Brand.Add(brand);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        //Edit method get
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = db.Brand.Find(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        //Edit method post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Update(brand);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        //Delete method get
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = db.Brand.Find(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        //Delete method post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id, Brand brand)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != brand.BrandID)
            {
                return NotFound();
            }
            var br = db.Brand.Find(id);
            if (br == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                db.Remove(br);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
    }
}
