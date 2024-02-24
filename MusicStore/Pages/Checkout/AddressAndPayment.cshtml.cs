using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Pages.Checkout
{

    [Authorize]
    public class AddressAndPaymentModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;

        private const string PROMO_CODE = "FREE";

        [BindProperty]
        public Order Order { get; set; }

        public AddressAndPaymentModel(MusicStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var formCollection = await HttpContext.Request.ReadFormAsync();

            try
            {
                if (string.Equals(formCollection["PromoCode"].FirstOrDefault(), PROMO_CODE,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return Page();
                }
                else
                {
                    Order.Username = HttpContext.User.Identity.Name;
                    Order.OrderDate = DateTime.Now;

                    //Add the Order
                    _dbContext.Orders.Add(Order);

                    //Process the order
                    var cart = Models.ShoppingCart.GetCart(_dbContext, HttpContext);
                    await cart.CreateOrder(Order);

                    // Save all changes
                    await _dbContext.SaveChangesAsync();

                    return RedirectToPage("Complete", new { id = Order.OrderId });
                }
            }
            catch
            {
                //Invalid - redisplay with errors
                return Page();
            }
        }
    }
}