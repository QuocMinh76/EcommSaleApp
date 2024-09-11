using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
                var searchBrand = db.Brand.FirstOrDefault(c => c.BrandName == brand.BrandName);
                if (searchBrand != null) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
                {
                    ViewBag.ExistErrorBrand = "This brand already exists";
                    return View(brand);
                }

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
                var searchBrand = db.Brand.AsNoTracking().FirstOrDefault(c => c.BrandName == brand.BrandName);
                if (searchBrand != null && searchBrand.BrandID != brand.BrandID) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
                {
                    ViewBag.ExistErrorBrand = "This brand already exists";
                    return View(brand);
                }

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

            if (TempData.ContainsKey("deleteError"))
            {
                // Pass the delete error message to the view using ViewBag
                ViewBag.DeleteError = TempData["deleteError"];
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

            var productsInBrand = await db.Product.AnyAsync(p => p.BrandID == id);
            if (productsInBrand)
            {
                // If there are products in this category, display a message
                TempData["deleteError"] = "Cannot delete brand. There are products associated with this brand.";
                return RedirectToAction(nameof(Delete), new { id });
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
