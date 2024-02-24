using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using MusicStore.Properties;

namespace MusicStore.Pages
{
    public class IndexModel : PageModel
    {

        public IList<Album> Albums { get; set; }

        private readonly MusicStoreContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly AppSettings _appSettings;

        public IndexModel(MusicStoreContext dbContext, IMemoryCache cache, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _cache = cache;
            _appSettings = appSettings.Value;
        }

        public async Task OnGetAsync()
        {
            var cacheKey = "topselling";
            List<Album> albums;
            if (!_cache.TryGetValue(cacheKey, out albums))
            {
                albums = await GetTopSellingAlbumsAsync(_dbContext, 6);

                if (albums != null && albums.Count > 0)
                {
                    if (_appSettings.CacheDbResults)
                    {
                        // Refresh it every 10 minutes.
                        // Let this be the last item to be removed by cache if cache GC kicks in.
                        _cache.Set(
                            cacheKey,
                            albums,
                            new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                            .SetPriority(CacheItemPriority.High));
                    }
                }
            }

            Albums = albums;
        }

        private Task<List<Album>> GetTopSellingAlbumsAsync(MusicStoreContext dbContext, int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            return dbContext.Albums
                .OrderByDescending(a => a.OrderDetails.Count)
                .Take(count)
                .ToListAsync();
        }
    }
}
