using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Pages.Store
{
    public class BrowseModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;

        [BindProperty(SupportsGet = true)]
        public string Genre { get; set; }
        public Genre GenreModel { get; set; }

        public BrowseModel(MusicStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            var genreModel = await _dbContext.Genres
                .Include(g => g.Albums)
                .Where(g => g.Name == Genre)
                .FirstOrDefaultAsync();

            GenreModel = genreModel;

            if (genreModel == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}