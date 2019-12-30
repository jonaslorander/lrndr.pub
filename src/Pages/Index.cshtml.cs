using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using lrndrpub.Data;
using lrndrpub.Models;

namespace lrndrpub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _um;
        private readonly AppDbContext _db;

        public List<Post> Posts { get; set; }
        public bool IsAdmin { get; set; } = false;

        public IndexModel(IConfiguration config, AppDbContext db, UserManager<AppUser> um)
        {
            _config = config;
            _db = db;
            _um = um;
        }

        // TODO: Add paging
        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
                IsAdmin = author.IsAdmin;
            }

            Posts = await _db.Posts.Where(p => !p.IsPage && ((p.IsPublished && p.PublishedAt <= DateTime.Now) || User.Identity.IsAuthenticated)).OrderByDescending(p => p.PublishedAt).ToListAsync();

            return Page();
        }
    }
}
