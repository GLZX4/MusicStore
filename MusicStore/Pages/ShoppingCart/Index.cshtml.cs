using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Models;

namespace MusicStore.Pages.ShoppingCart
{
    public class IndexModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;

        public IEnumerable<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }

        public IndexModel(MusicStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGet()
        {
            var cart = Models.ShoppingCart.GetCart(_dbContext, HttpContext);

            CartItems = await cart.GetCartItems();
            CartTotal = await cart.GetTotal();
        }

        public async Task<IActionResult> OnGetAddToCartAsync(int id)
        {
            // Retrieve the album from the database
            var addedAlbum = await _dbContext.Albums
                .SingleAsync(album => album.AlbumId == id);

            // Add it to the shopping cart
            var cart = Models.ShoppingCart.GetCart(_dbContext, HttpContext);

            await cart.AddToCart(addedAlbum);

            await _dbContext.SaveChangesAsync();

            // Go back to the main store page for more shopping
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostRemoveFromCart(int id)
        {
            // Retrieve the current user's shopping cart
            var cart = Models.ShoppingCart.GetCart(_dbContext, HttpContext);

            // Get the name of the album to display confirmation
            var cartItem = await _dbContext.CartItems
                .Where(item => item.CartItemId == id)
                .Include(c => c.Album)
                .SingleOrDefaultAsync();

            string message;
            int itemCount;
            if (cartItem != null)
            {
                // Remove from cart
                itemCount = cart.RemoveFromCart(id);

                await _dbContext.SaveChangesAsync();

                string removed = (itemCount > 0) ? " 1 copy of " : string.Empty;
                message = removed + cartItem.Album.Title + " has been removed from your shopping cart.";
            }
            else
            {
                itemCount = 0;
                message = "Could not find this item, nothing has been removed from your shopping cart.";
            }

            // Display the confirmation message

            var results = new ShoppingCartRemoveViewModel
            {
                Message = message,
                CartTotal = await cart.GetTotal(),
                CartCount = await cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return new JsonResult(results);
        }

        public class ShoppingCartViewModel
        {
            public List<CartItem> CartItems { get; set; }
            public decimal CartTotal { get; set; }
        }

        public class ShoppingCartRemoveViewModel
        {
            public string Message { get; set; }
            public decimal CartTotal { get; set; }
            public int CartCount { get; set; }
            public int ItemCount { get; set; }
            public int DeleteId { get; set; }
        }
    }
}