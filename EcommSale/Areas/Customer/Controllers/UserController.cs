using EcommSale.Data;
using EcommSale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommSale.Areas.Customer.Controllers
{
	[Area("Customer")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
	{
		UserManager<ApplicationUser> _userManager;
		ApplicationDbContext _db;
		public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
		{
			_userManager = userManager;
			_db = db;
		}
		public IActionResult Index()
		{
			return View(_db.ApplicationUsers.ToList());
		}

		// Get Details action method
		public async Task<IActionResult> Details(string id)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		// Get Lockout action method
		public async Task<IActionResult> Lockout(string id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var user = _db.ApplicationUsers.FirstOrDefault(y => y.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			if (TempData.ContainsKey("adminLockError"))
			{
				// Pass the delete error message to the view using ViewBag
				ViewBag.AdminLockError = TempData["adminLockError"];
			}

			return View(user);
		}

		// Post Lockout action method
		[HttpPost]
		public async Task<IActionResult> Lockout(ApplicationUser user, string id)
		{
			var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
			if (userInfo == null)
			{
				return NotFound();
			}

			// Check if the user being locked out is an admin user
			if (await _userManager.IsInRoleAsync(userInfo, "Admin"))
			{
				// Retrieve all users from the database
				var allUsers = await _userManager.Users.ToListAsync();

				// Count the number of active admin users in memory
				var activeAdminCount = allUsers.Count(u => u.LockoutEnd == null && _userManager.IsInRoleAsync(u, "Admin").Result);

				// If there's only one active admin user left, return an error
				if (activeAdminCount <= 1)
				{
					TempData["adminLockError"] = "Cannot lock out the only active admin user.";
					return RedirectToAction(nameof(Lockout), new { id });
				}
			}

			userInfo.LockoutEnd = DateTime.Now.AddYears(100);
			int rowAffected = _db.SaveChanges();
			if (rowAffected > 0)
			{
				TempData["lockout"] = "User has been locked out";
				return RedirectToAction(nameof(Index));
			}
			return View(userInfo);
		}

		// Get Active action method
		public async Task<IActionResult> Active(string id)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		// Post Active action method
		[HttpPost]
		public async Task<IActionResult> Active(ApplicationUser user)
		{
			var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
			if (userInfo == null)
			{
				return NotFound();
			}
			userInfo.LockoutEnd = DateTime.Now.AddDays(-1);
			int rowAffected = _db.SaveChanges();
			if (rowAffected > 0)
			{
				TempData["lockout"] = "User has been reactivated";
				return RedirectToAction(nameof(Index));
			}
			return View(userInfo);
		}

		// Get Delete action method
		public async Task<IActionResult> Delete(string id)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		// Post Delete action method
		[HttpPost]
		public async Task<IActionResult> Delete(ApplicationUser user)
		{
			var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
			if (userInfo == null)
			{
				return NotFound();
			}
			_db.ApplicationUsers.Remove(userInfo);
			int rowAffected = _db.SaveChanges();
			if (rowAffected > 0)
			{
				TempData["delete"] = "User has been deleted";
				return RedirectToAction(nameof(Index));
			}
			return View(userInfo);
		}
	}
}
