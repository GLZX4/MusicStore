using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Pages.Checkout
{
    public class CompleteModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;

        public CompleteModel(MusicStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = HttpContext.User.Identity.Name;

            // Validate customer owns this order
            bool isValid = await _dbContext.Orders.AnyAsync(
                o => o.OrderId == Id &&
                o.Username == userName);

            if (isValid)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }
    }
}