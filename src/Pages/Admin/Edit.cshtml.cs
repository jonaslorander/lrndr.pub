using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using lrndrpub.Data;
using lrndrpub.Models;
using lrndrpub.Services;

using Slugify;

namespace lrndrpub.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _um;
        private readonly AppDbContext _db;
        private readonly IFileService _fs;

        [BindProperty]
        public Post Post { get; set; }

        [BindProperty]
        public IFormFile CoverImage { get; set; }

        public bool IsAdmin { get; set; }

        public EditModel(IConfiguration config, AppDbContext db, UserManager<AppUser> um, IFileService fs)
        {
            _config = config;
            _db = db;
            _um = um;
            _fs = fs;
        }

        public async Task OnGetAsync(int id = 0)
        {
            var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
            IsAdmin = author.IsAdmin;

            Post = new Post();

            if (id > 0)
            {
                Post = await _db.Posts.SingleAsync(p => p.PostId == id);
            }
            else
            {
                Post.PublishedAt = DateTime.Now;
                //Post.Slug = "dummy";
                Post.CommentsOpen = bool.Parse(_config["EnableComments"]);
            }
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
            if (CoverImage != null)
            {
                if (!_fs.ValidateImageFile(CoverImage))
                {
                    ModelState.TryAddModelError("CoverImage_Type", "The CoverImage is not a valid image.");
                }

                // Is image less the 2MB?
                // TODO: Parameterized size
                if (CoverImage.Length > 2097152)
                {
                    ModelState.TryAddModelError("CoverImage_Size", "The CoverImage file size is too big.");
                }
            }

            if (!ModelState.IsValid)
                return Page();

            if (CoverImage != null)
            {
                // Save CoverImage
                using (var ms = new MemoryStream())
                {
                    await CoverImage.CopyToAsync(ms);

                    // Is file less then 2 MB?
                    // TODO: Parameterize length. in AppSettings.json or configurable through databse?
                    // IS this check really necessary if we check the length above?
                    if (ms.Length <= 2097152)
                    {
                        Post.CoverImage = await _fs.SaveFile(ms.ToArray(), CoverImage.FileName);
                    }
                }
            }

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

            // Did the user provide a slug, and is this a new post?
            if( Post.Slug != string.Empty && Post.PostId == 0)
            {
                // Check if slug is already in use
                Post.Slug = await CheckSlug(Post.Slug);
            }
            else
            {
                // User did not provide a slug (or removed the slug on an existing post)
                if (Post.Slug == string.Empty)
                {
                    Post.Slug = await CreateSlug(Post.Title);
                }
            }

            await SaveFilesToDisk(Post);
            _db.Posts.Update(Post);
            await _db.SaveChangesAsync();

            return Redirect(String.Format("/{0}/{1}", (Post.IsPage ? "page" : "post"), Post.Slug));
        }

        private async Task SaveFilesToDisk(Post post)
        {
            var imgRegex = new Regex("<img[^>]+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var base64Regex = new Regex("data:[^/]+/(?<ext>[a-z]+);base64,(?<base64>.+)", RegexOptions.IgnoreCase);
            
            foreach (Match match in imgRegex.Matches(post.Content))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<root>" + match.Value + "</root>");

                var img = doc.FirstChild.FirstChild;
                var srcNode = img.Attributes["src"];
                var fileNameNode = img.Attributes["data-filename"];

                // The HTML editor creates base64 DataURIs which we'll have to convert to image files on disk
                if (srcNode != null && fileNameNode != null)
                {
                    string extension = System.IO.Path.GetExtension(fileNameNode.Value);

                    // Only accept image files
                    if (!_fs.ValidateImageFile(extension))
                    {
                        continue;
                    }

                    var base64Match = base64Regex.Match(srcNode.Value);
                    if (base64Match.Success)
                    {
                        byte[] bytes = Convert.FromBase64String(base64Match.Groups["base64"].Value);
                        srcNode.Value = await _fs.SaveFile(bytes, fileNameNode.Value).ConfigureAwait(false);

                        img.Attributes.Remove(fileNameNode);
                        post.Content = post.Content.Replace(match.Value, img.OuterXml);
                    }
                }
            }
        }
        private async Task<string> CheckSlug(string slug)
        {
            string s = slug;
            int i = 1;

            // Generate slug and search for duplicates
            while ((await _db.Posts.CountAsync(p => p.Slug == s)) > 0)
            {
                s = slug + "-" + i;
                i++;
            }

            return s;
        }

        /*
         * TODO: Javascript som anropar denna function som en GET när man skriver titel
         */
        private async Task<string> CreateSlug(string slug)
        {
            string s;
            int i = 1;

            SlugHelper helper = new SlugHelper();
            s = helper.GenerateSlug(slug);

            // Generate slug and search for duplicates
            while ((await _db.Posts.CountAsync(p => p.Slug == s)) > 0)
            {
                s = helper.GenerateSlug(slug + "-" + i);
                i++;
            }

            return s;
        }
    }
}