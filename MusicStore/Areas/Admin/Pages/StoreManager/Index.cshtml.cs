using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Areas.Admin.Pages.StoreManager
{

    [Authorize("ManageStore")]
    public class IndexModel : PageModel
    {

        private readonly MusicStoreContext _dbContext;

        public IEnumerable<Album> Albums { get; set; }

        public IndexModel(MusicStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            var albums = await _dbContext.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .ToListAsync();

            Albums = albums;
        }
    }
}