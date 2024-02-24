using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;

namespace MusicStore.Areas.Admin.Pages.StoreManager
{

    [Authorize("ManageStore")]
    public class RemoveAlbumModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;
        private readonly IMemoryCache _cache;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Album Album { get; set; }

        public RemoveAlbumModel(MusicStoreContext dbContext, IMemoryCache cache)
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var album = await _dbContext.Albums.Where(a => a.AlbumId == Id).FirstOrDefaultAsync();

            if (album == null)
            {
                return NotFound();
            }

            _dbContext.Albums.Remove(album);
            await _dbContext.SaveChangesAsync();
            //Remove the cache entry as it is removed
            _cache.Remove(GetCacheKey(Id));

            return RedirectToPage("Index");
        }

        private static string GetCacheKey(int id)
        {
            return string.Format("album_{0}", id);
        }
    }
}