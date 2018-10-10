using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using lrndrpub.Data;
using lrndrpub.Models;

namespace lrndrpub.Pages.Admin
{
    public class ProfileModel : PageModel
    {
        public AppDbContext _db;
        public UserManager<AppUser> _um;

        public bool IsAdmin { get; set; }

        [BindProperty]
        public Author Author { get; set; }

        public ProfileModel(AppDbContext db, UserManager<AppUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> OnGet(string id = null)
        {
            var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
            IsAdmin = author.IsAdmin;

            if (id == null)
                id = (await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name)).AuthorId.ToString();

            Author = await _db.Authors.SingleAsync(a => a.AuthorId == uint.Parse(id));

            if (Author == null)
                return NotFound();

            return Page();
        }
    }
}