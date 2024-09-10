using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Identity.Pages.Account.Manage
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public Order Order { get; set; }
        public IList<OrderDetails> OrderDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Order = await _db.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderID == id);
            if (Order == null)
            {
                return NotFound();
            }

            OrderDetails = await _db.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderID == id)
                .ToListAsync();

            return Page();
        }
    }
}

