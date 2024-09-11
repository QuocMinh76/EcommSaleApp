using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IHostingEnvironment _he;

        public ProductController(ApplicationDbContext db, IHostingEnvironment he)
        {
            _db = db;
            _he = he;
        }
        public IActionResult Index()
        {
            return View(_db.Product.Include(c=>c.Category).Include(b=>b.Brand).ToList());
        }
        
        //Create get action method
        public ActionResult Create()
        {
            ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
            ViewData["brandID"] = new SelectList(_db.Brand.ToList(), "BrandID", "BrandName");
            return View();
        }

        //Create post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, IFormFile image)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var searchProduct = _db.Product.FirstOrDefault(c => c.ProductName == product.ProductName);
                if (searchProduct != null) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
                {
                    ViewBag.ExistError = "This product already exists";
                    ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
                    ViewData["brandID"] = new SelectList(_db.Brand.ToList(), "BrandID", "BrandName");
                    return View();
                }

                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    product.Image = "Images/" + image.FileName;
                }

                if (image == null)
                {
                    product.Image = "Images/noimage.png";
                }

                product.ProductCount = 0;

                _db.Product.Add(product);
                await _db.SaveChangesAsync();
                TempData["create"] = "Product has been created";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        
        // Get Edit action Method
        public ActionResult Edit(int? id)
        {
            ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
            ViewData["brandID"] = new SelectList(_db.Brand.ToList(), "BrandID", "BrandName");
            if (id == null)
            {
                return NotFound();
            }

            var product = _db.Product.Include(c => c.Category).Include(b => b.Brand).FirstOrDefault(c => c.ProductID == id);
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
            var findProd = _db.Product.Where(c => c.ProductID == product.ProductID).AsNoTracking().FirstOrDefault();

            var searchProduct = _db.Product.AsNoTracking().FirstOrDefault(c => c.ProductName == product.ProductName);
            if (searchProduct != null && searchProduct.ProductID != product.ProductID) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
            {
                ViewBag.ExistError = "This product already exists";
                ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
                ViewData["brandID"] = new SelectList(_db.Brand.ToList(), "BrandID", "BrandName");
                return View(product);
            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    product.Image = "Images/" + image.FileName;
                }

                if (image == null)
                {
                    product.Image = findProd.Image;
                }

                _db.Update(product);
                await _db.SaveChangesAsync();
                TempData["edit"] = "Product has been updated";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        //Delete method get
        public ActionResult Delete(int? id)
        {
            ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
            ViewData["brandID"] = new SelectList(_db.Brand.ToList(), "BrandID", "BrandName");
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Product.Include(c => c.Category).Include(b => b.Brand).Where(c => c.ProductID == id).FirstOrDefault(c => c.ProductID == id);
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
            var pr = _db.Product.FirstOrDefault(c => c.ProductID == id);
            if (pr == null)
            {
                return NotFound();
            }
            
            _db.Remove(pr);
            await _db.SaveChangesAsync();
            TempData["delete"] = "Product has been deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
