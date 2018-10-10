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
    public class UsersModel : PageModel
    {
        private AppDbContext _db;
        private UserManager<AppUser> _um;

        public bool IsAdmin { get; set; }

        [BindProperty]
        public IList<Author> Users { get; set; }

        public UsersModel(AppDbContext db, UserManager<AppUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
            IsAdmin = author.IsAdmin;

            if (!IsAdmin)
                return NotFound(); // RedirectToPage("/Error");

            Users = await _db.Authors.ToListAsync();
            
            return Page();
        }
    }
}