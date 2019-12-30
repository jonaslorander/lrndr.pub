using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using lrndrpub.Data;
using lrndrpub.Models;

namespace lrndrpub.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _um;
        private AppDbContext _db;

        public bool IsAdmin { get; set; }

        public IndexModel(IConfiguration config, AppDbContext db, UserManager<AppUser> um)
        {
            _config = config;
            _db = db;
            _um = um;
        }

        public async Task OnGetAsync()
        {
            var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
            IsAdmin = author.IsAdmin;
        }
    }
}
