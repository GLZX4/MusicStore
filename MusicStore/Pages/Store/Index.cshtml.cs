using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using MusicStore.Properties;

namespace MusicStore.Pages.Store
{
    public class IndexModel : PageModel
    {

        private readonly AppSettings _appSettings;

        private readonly MusicStoreContext _dbContext;

        public IEnumerable<Genre> Genres { get; set; }

        public IndexModel(IOptions<AppSettings> appSettings, MusicStoreContext dbContext)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            var genres = await _dbContext.Genres.ToListAsync();

            Genres = genres;
        }
    }
}