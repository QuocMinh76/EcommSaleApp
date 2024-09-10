// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using EcommSale.Data;
using EcommSale.Models;
using EcommSale.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EcommSale.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
		private readonly ApplicationDbContext _db;

		public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
			// Retrieve cart from session
			var cartItems = HttpContext.Session.Get<List<CartItemVm>>("cartItems");

			if (cartItems != null)
			{
				foreach (var cartItem in cartItems)
				{
					// Retrieve the product from the database
					var product = _db.Product.FirstOrDefault(p => p.ProductID == cartItem.Product.ProductID);
					if (product != null)
					{
						// Update the stock by adding back the cartItem quantity
						product.ProductCount += cartItem.Quantity;
					}
				}
				// Save changes to the database
				await _db.SaveChangesAsync();
			}

			// Clear the cart
			HttpContext.Session.Remove("cartItems");

			await _signInManager.SignOutAsync();
            HttpContext.Session.SetString("roleName", "");
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
