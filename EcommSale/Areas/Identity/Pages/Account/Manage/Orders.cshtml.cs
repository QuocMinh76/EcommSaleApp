using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Identity.Pages.Account.Manage
{
    public class OrdersModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _db;

		public OrdersModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
		{
			_userManager = userManager;
			_db = db;
		}

		public IList<Order> Orders { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			// Get the currently logged-in user
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			// Fetch the orders for this user
			Orders = await _db.Order
				.Where(o => o.UserID == user.Id)
				.ToListAsync();

			return Page();
		}


	}
}
