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

namespace lrndrpub.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _um;
        private AppDbContext _db;

        [BindProperty]
        public Post Post { get; set; }

        public bool IsAdmin { get; set; }

        public EditModel(IConfiguration config, AppDbContext db, UserManager<AppUser> um)
        {
            _config = config;
            _db = db;
            _um = um;
        }

        public async Task OnGetAsync(int id = 0)
        {
            var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
            IsAdmin = author.IsAdmin;

            Post = new Post();

            if (id > 0)
                Post = await _db.Posts.SingleAsync(p => p.PostId == id);
        }

        public async Task<IActionResult> OnPostPublishAsync()
        {
            return await SaveChanges();
        }

        public async Task<IActionResult> OnPostSaveDraftAsync()
        {
            return await SaveChanges(false);
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            Post post = await _db.Posts.SingleAsync(p => p.PostId == Post.PostId);
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();

            return Redirect("~/Admin/Index");
        }

        public IActionResult OnPostCancel()
        {
            return Redirect("~/Admin/Index");
        }

        private async Task<IActionResult> SaveChanges(bool pub = true)
        {
            if (!ModelState.IsValid)
                return Page();

            var AuthorId = (await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name)).AuthorId;

            // This will keep the original publisher
            var pubAuthorId = AuthorId;
            if (Post.IsPublished)
                pubAuthorId = Post.PublishedBy;

            Post.IsPublished = pub;
            Post.PublishedBy = pub ? pubAuthorId : 0;

            if (Post.PostId > 0)
            {
                // Edit of old post
                Post.UpdatedBy = AuthorId;
                Post.UpdatedAt = DateTime.Now;
            }
            else
            {
                // This is a new post
                Post.CreatedBy = AuthorId;
                Post.CreatedAt = DateTime.Now;
            }

            Post.Slug = await GetSlug(Post.PostId, Post.Slug);

            _db.Posts.Update(Post);
            await _db.SaveChangesAsync();

            return Redirect(String.Format("/{0}/{1}", (Post.IsPage ? "page" : "post"), Post.Slug));
        }

        private async Task<string> GetSlug(uint pid, string slug)
        {
            // TODO: Check slug conflicts

            return await Task.FromResult(slug);
        }
    }
}