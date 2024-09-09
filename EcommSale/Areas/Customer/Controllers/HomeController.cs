using EcommSale.Data;
using EcommSale.Models;
using EcommSale.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace EcommSale.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index(string productName, decimal? minPrice, decimal? maxPrice, int? brand, int? category, int? page)
        {
            //Load query sản phẩm
            var query = _db.Product.Include(c => c.Category).Include(f => f.Brand).AsQueryable();

            // Kiểm tra các tham số đầu vào tìm kiếm
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(p => p.ProductName.Contains(productName));
            }

            if (minPrice != null)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.CategoryID == category);
            }

            if (brand.HasValue)
            {
                query = query.Where(p => p.BrandID == brand);
            }

            // Chuyển cái query thành một danh sách có pageList
            var products = query.ToList().ToPagedList(page ?? 1, 9);

            // Chuyển các giá trị của mỗi loại sang bên view
            ViewBag.Categories = _db.Category.ToList();
            ViewBag.Brands = _db.Brand.ToList();

            // Trả ds sản phẩm về trang hiển thị
            return View(products);
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

        //Get product detail method
        public async Task<ActionResult> Details(int? id)
        {
            // Kiểm tra sản phẩm có tồn tại
            if (id == null)
            {
                return NotFound();
            }

            var product = _db.Product.Include(c => c.Category).Include(c => c.Brand).FirstOrDefault(c => c.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Kiểm tra xem tk login có được bình luận k
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                // Check if the user is an admin
                bool isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

                // If the user is not an admin, check if they have purchased the product
                if (!isAdmin)
                {
                    // Query the database to check if the user has purchased the product
                    bool hasPurchased = _db.OrderDetails
                        .Any(od => od.Order.UserID == currentUser.Id && od.ProductID == id);

                    // If the user has not purchased the product, disable the comment submission button
                    if (!hasPurchased)
                    {
                        ViewBag.CanComment = false;
                    }
                }
            } 

            //Lấy comment hiện có của sp
            var comments = _db.Comment
            .Where(c => c.ProductID == id)
            .ToList();

            ViewBag.Comments = comments;
            ViewBag.CanComment ??= true; // If ViewBag.CanComment is not set, default to true
            
            return View(product);
        }

		//Post product detail method
		[HttpPost]
		[ActionName("Details")]
        public ActionResult ProductDetail(int? id, int quantity)
        {
            if (id == null)
            {
                return NotFound();
            }
			var product = _db.Product.Include(c => c.Category).Include(c => c.Brand).FirstOrDefault(c => c.ProductID == id);
			if (product == null)
			{
				return NotFound();
			}
            var cartItems = HttpContext.Session.Get<List<CartItemVm>>("cartItems");
            if (cartItems == null) 
            {
                cartItems = new List<CartItemVm>();
            }
            var existingCartItem = cartItems.FirstOrDefault(c=>c.Product.ProductID == id);
			if (existingCartItem != null)
			{
				// If the product is already in the cart, just increase its quantity
				existingCartItem.Quantity += quantity;
			}
			else
			{
				// If the product is not in the cart, add it as a new cart item
				cartItems.Add(new CartItemVm
				{
					Product = product,
					Quantity = quantity
				});
			}

			TempData["add"] = "Added to cart";

			HttpContext.Session.Set("cartItems", cartItems);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
        public IActionResult Remove(int ?id)
        {
			var cartItems = HttpContext.Session.Get<List<CartItemVm>>("cartItems");
			if (cartItems != null)
			{
				var cartItem = cartItems.FirstOrDefault(c => c.Product.ProductID == id);
				if (cartItem != null)
				{
					cartItems.Remove(cartItem);
					HttpContext.Session.Set("cartItems", cartItems);
				}
			}
			return RedirectToAction(nameof(Index));
		}

        //Get remove action method
        [ActionName("Remove")]
        public IActionResult RemoveFromCart(int? id)
        {
            List<CartItemVm> cartItems = HttpContext.Session.Get<List<CartItemVm>>("cartItems");
            if (cartItems != null) 
            {
                var cartItem = cartItems.FirstOrDefault(item => item.Product.ProductID == id);
                if (cartItem != null) 
                {
                    cartItem.Quantity--;
                    if (cartItem.Quantity <= 0) 
                    {
                        cartItems.Remove(cartItem);
                    }
                    HttpContext.Session.Set("cartItems", cartItems);
                    if (cartItems.Count == 0)
                    {
                        return RedirectToAction(nameof(Index), "Home");
                    }
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        //Get product cart action method
        public IActionResult Cart()
        {
			var cartItems = HttpContext.Session.Get<List<CartItemVm>>("cartItems");
			if (cartItems == null)
			{
				cartItems = new List<CartItemVm>();
			}
			return View(cartItems); 

        }

        // Found similar products
        public IActionResult SimilarProducts(int productId)
        {
            var product = _db.Product.Include(p => p.Category).Include(p => p.Brand).FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
            {
                return NotFound();
            }

            var similarProducts = _db.Product
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != productId)
                .ToList();

            var similarProductsSorted = similarProducts
                .Select(p => new
                {
                    Product = p,
                    Similarity = ManhattanDistance(product, p)
                })
                .OrderBy(s => s.Similarity)
                .Take(4)
                .Select(s => s.Product)
                .ToList();

            return PartialView("_SimilarProductsPartial", similarProductsSorted);
        }

        private double ManhattanDistance(Product product1, Product product2)
        {
            // Tính toán giá trị min và max cho mỗi thuộc tính
            var minPrice = _db.Product.Min(p => p.Price);
            var maxPrice = _db.Product.Max(p => p.Price);

            var prod1Price = (product1.Price - minPrice) / (maxPrice - minPrice);
            var prod2Price = (product2.Price - minPrice) / (maxPrice - minPrice);

            double distance = (double)Math.Abs(prod2Price - prod1Price);
            // Nếu cả hai sản phẩm có cùng Brand
            if (product1.Brand.BrandName == product2.Brand.BrandName)
            {
                distance += 0; // Trả về khoảng cách 0
            }
            else
            {
                distance += 1; // Trả về khoảng cách 1
            }
            // Color
            if (product1.ProductColor == product2.ProductColor)
            {
                distance += 0; // Trả về khoảng cách 0
            }
            else
            {
                distance += 1; // Trả về khoảng cách 1
            }

            return distance;
        }

        //comment
        [HttpPost]
        public async Task<ActionResult> AddComment(int productId, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            string commenterName;
            if (!string.IsNullOrEmpty(currentUser.FirstName) && !string.IsNullOrEmpty(currentUser.LastName))
            {
                commenterName = $"{currentUser.FirstName} {currentUser.LastName}";
            }
            else if (!string.IsNullOrEmpty(currentUser.FirstName))
            {
                commenterName = currentUser.FirstName;
            }
            else if (!string.IsNullOrEmpty(currentUser.LastName))
            {
                commenterName = currentUser.LastName;
            }
            else
            {
                commenterName = currentUser.UserName;
            }

            if (string.IsNullOrEmpty(content))
            {
                // Handle empty comment content (optional)
                TempData["ErrorMessage"] = "Comment content cannot be empty.";
                return RedirectToAction("Details", new { id = productId });
            }

            var comment = new Comment
            {
                CommenterID = currentUser.Id,
                CommenterName = commenterName,
                ProductID = productId,
                Content = content,
                PostedDate = DateTime.Now
            };

            // Add the comment to the database
            _db.Comment.Add(comment);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Comment posted!";
            TempData["CommentPosted"] = true;

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        public IActionResult DeleteComment(int? commentId)
        {
            if (commentId == null)
            {
                return NotFound();
            }

            var comment = _db.Comment.Find(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to delete the comment
            if (!User.IsInRole("Admin") && comment.CommenterID != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                return Forbid(); // User is not authorized to delete the comment
            }

            _db.Comment.Remove(comment);
            _db.SaveChanges();

            TempData["DeleteMessage"] = "Comment deleted!";

            return RedirectToAction("Details", new { id = comment.ProductID });
        }
    }
}
