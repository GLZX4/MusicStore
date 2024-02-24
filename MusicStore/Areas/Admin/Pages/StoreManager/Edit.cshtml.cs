using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;

namespace MusicStore.Areas.Admin.Pages.StoreManager
{

    [Authorize("ManageStore")]
    public class EditModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;
        private readonly IMemoryCache _cache;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public SelectList GenreId { get; set; }
        public SelectList ArtistId { get; set; }

        [BindProperty]
        public Album Album { get; set; }

        public EditModel(MusicStoreContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var album = await _dbContext.Albums.Where(x => x.AlbumId == Id).FirstOrDefaultAsync();

            if (album == null)
            {
                return NotFound();
            }

            Album = album;

            GenreId = new SelectList(_dbContext.Genres, "GenreId", "Name", Album.GenreId);
            ArtistId = new SelectList(_dbContext.Artists, "ArtistId", "Name", Album.ArtistId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                _dbContext.Update(Album);
                await _dbContext.SaveChangesAsync();
                //Invalidate the cache entry as it is modified
                _cache.Remove(GetCacheKey(Album.AlbumId));
                return RedirectToPage("Index");
            }

            GenreId = new SelectList(_dbContext.Genres, "GenreId", "Name", Album.GenreId);
            ArtistId = new SelectList(_dbContext.Artists, "ArtistId", "Name", Album.ArtistId);

            return Page();
        }

        private static string GetCacheKey(int id)
        {
            return string.Format("album_{0}", id);
        }
    }
}