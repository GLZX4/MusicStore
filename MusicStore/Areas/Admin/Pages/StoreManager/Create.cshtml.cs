using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;

namespace MusicStore.Areas.Admin.Pages.StoreManager
{
    [Authorize("ManageStore")]
    public class CreateModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;
        private readonly IMemoryCache _cache;

        public SelectList GenreId { get; set; }
        public SelectList ArtistId { get; set; }

        [BindProperty]
        public Album Album { get; set; }

        public CreateModel(MusicStoreContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public void OnGet()
        {
            GenreId = new SelectList(_dbContext.Genres, "GenreId", "Name");
            ArtistId = new SelectList(_dbContext.Artists, "ArtistId", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                _dbContext.Albums.Add(Album);
                await _dbContext.SaveChangesAsync();


                _cache.Remove("latestAlbum");

                return RedirectToPage("Index");
            }

            GenreId = new SelectList(_dbContext.Genres, "GenreId", "Name", Album.GenreId);
            ArtistId = new SelectList(_dbContext.Artists, "ArtistId", "Name", Album.ArtistId);

            return Page();
        }
    }
}