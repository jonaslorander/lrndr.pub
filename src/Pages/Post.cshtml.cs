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
    public class PostModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _um;
        private AppDbContext _db;

        public Post Post { get; set; }
        public List<Comment> Comments { get; set; }
        public bool IsAdmin { get; set; } = false;

        public PostModel(IConfiguration config, AppDbContext db, UserManager<AppUser> um)
        {
            _config = config;
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (User.Identity.IsAuthenticated)
            {
                var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
                IsAdmin = author.IsAdmin;
            }

            if (!string.IsNullOrEmpty(slug) && (await _db.Posts.AnyAsync(p => p.Slug == slug && !p.IsPage && ((p.IsPublished && p.PublishedAt <= DateTime.Now) || User.Identity.IsAuthenticated))))
            {
                Post = await _db.Posts.SingleAsync(p => p.Slug == slug);
                Comments = await _db.Comments.Where(c => c.PostId == Post.PostId && (c.IsPublished || User.Identity.IsAuthenticated)).ToListAsync();
                
                return Page();
            }
            else
                return NotFound();
        }
    }
}