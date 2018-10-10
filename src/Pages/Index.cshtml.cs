using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using lrndrpub.Data;
using lrndrpub.Models;
using lrndrpub.Services;

namespace lrndrpub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public IndexModel(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public void OnGet()
        {

        }

        public void OnPostChangeTitle()
        {
            // Update title setting
            _config["Title"] = ("New title " + DateTime.Now.Ticks);
        }
    }
}
