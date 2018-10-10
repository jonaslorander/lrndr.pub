using System;
using System.Collections.Generic;
using System.IO;
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
    public class ThemeCssModel : PageModel
    {
        private readonly IConfiguration _config;

        public ThemeCssModel(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult OnGet(string type)
        {
            if (!(type.Contains("css") || type.Contains("js")))
                return NotFound();

            // TODO: Minify 
            if (type.Contains("min"))
                type = type.Remove(0, 4);

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Pages", "Themes", _config["Theme"], "theme." + type);


            return PhysicalFile(file, "text/css");
        }
    }
}
