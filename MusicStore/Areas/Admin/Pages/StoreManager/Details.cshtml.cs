using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using MusicStore.Properties;

namespace MusicStore.Areas.Admin.Pages.StoreManager
{

    [Authorize("ManageStore")]
    public class DetailsModel : PageModel
    {

        private readonly IMemoryCache _cache;
        private readonly MusicStoreContext _dbContext;
        private readonly AppSettings _appSettings;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Album Album { get; set; }

        public DetailsModel(IMemoryCache cache, MusicStoreContext musicStoreContext, IOptions<AppSettings> appSettings)
        {
            _cache = cache;
            _dbContext = musicStoreContext;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var cacheKey = GetCacheKey(Id);

            Album album;
            if (!_cache.TryGetValue(cacheKey, out album))
            {
                album = await _dbContext.Albums
                        .Where(a => a.AlbumId == Id)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .FirstOrDefaultAsync();

                if (album != null)
                {
                    if (_appSettings.CacheDbResults)
                    {
                        //Remove it from cache if not retrieved in last 10 minutes.
                        _cache.Set(
                            cacheKey,
                            album,
                            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    }
                }
            }

            if (album == null)
            {
                _cache.Remove(cacheKey);
                return NotFound();
            }

            Album = album;

            return Page();
        }

        private static string GetCacheKey(int id)
        {
            return string.Format("album_{0}", id);
        }
    }
}