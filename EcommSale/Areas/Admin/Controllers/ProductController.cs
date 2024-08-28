using EcommSale.Data;
using ECommSale.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext db;

        public ProductController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View(db.Product.Include(c=>c.Category).Include(b=>b.Brand).ToList());
        }
        
        //Post Index action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(decimal? lowAmount, decimal? highAmount, string? productName)
        {
            var query = db.Product.Include(c => c.Category).Include(b => b.Brand).AsQueryable();

            if (productName != null)
            {
                productName = productName.Trim();
                query = query.Where(p => p.ProductName.ToLower().Contains(productName.ToLower()));
            }

            if (lowAmount != null)
            {
                query = query.Where(p => p.Price >= lowAmount);
            }

            if (highAmount != null)
            {
                query = query.Where(p => p.Price <= highAmount);
            }
            var products = query.ToList();

            return View(products);
        }
        
        //Create get action method
        public ActionResult Create()
        {
            ViewData["categoryID"] = new SelectList(db.Category.ToList(), "CategoryID", "CategoryName");
            ViewData["brandID"] = new SelectList(db.Brand.ToList(), "BrandID", "BrandName");
            return View();
        }

        //Create post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                db.Product.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        
        // Get Edit action Method
        public ActionResult Edit(int? id)
        {
            ViewData["categoryID"] = new SelectList(db.Category.ToList(), "CategoryID", "CategoryName");
            ViewData["brandID"] = new SelectList(db.Brand.ToList(), "BrandID", "BrandName");
            if (id == null)
            {
                return NotFound();
            }

            var product = db.Product.Include(c => c.Category).Include(b => b.Brand).FirstOrDefault(c => c.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Post Edit action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile image)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                db.Update(product);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        //Delete method get
        public ActionResult Delete(int? id)
        {
            ViewData["categoryID"] = new SelectList(db.Category.ToList(), "CategoryID", "CategoryName");
            ViewData["brandID"] = new SelectList(db.Brand.ToList(), "BrandID", "BrandName");
            if (id == null)
            {
                return NotFound();
            }
            var product = db.Product.Include(c => c.Category).Include(b => b.Brand).Where(c => c.ProductID == id).FirstOrDefault(c => c.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //Delete method post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pr = db.Product.FirstOrDefault(c => c.ProductID == id);
            if (pr == null)
            {
                return NotFound();
            }
            
            db.Remove(pr);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
