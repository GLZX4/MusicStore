using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using MusicStore.Properties;

namespace MusicStore.Pages.Store
{
    public class DetailsModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly AppSettings _appSettings;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Album Album { get; set; }

        public DetailsModel(MusicStoreContext dbContext, IMemoryCache cache, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _cache = cache;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var cacheKey = string.Format("album_{0}", Id);
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
                        //Remove it from cache if not retrieved in last 10 minutes
                        _cache.Set(
                            cacheKey,
                            album,
                            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    }
                }
            }

            if (album == null)
            {
                return NotFound();
            }

            Album = album;

            return Page();
        }
    }
}