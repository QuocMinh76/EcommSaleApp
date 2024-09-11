using EcommSale.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var orders = _db.Order
                .Include(o => o.User)
                .ToList();

            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _db.Order.Include(o => o.User).FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = _db.OrderDetails
                                    .Where(od => od.OrderID == order.OrderID)
                                    .Include(od => od.Product)
                                    .ToList();

            ViewBag.OrderDetails = orderDetails;
            return View(order);
        }
    }
}
