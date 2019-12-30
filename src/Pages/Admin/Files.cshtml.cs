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

namespace lrndrpub.Pages.Admin
{
    public class FilesModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _um;
        private readonly AppDbContext _db;
        private readonly IFileService _fs;

        public bool IsAdmin { get; set; }

        public List<FileInfo> Images { get; set; }

        [BindProperty]
        public string SelectedFile { get; set; }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        [ViewData]
        public bool SuccessShow { get; private set; }

        [ViewData]
        public string SuccessMessage { get; private set; }

        public FilesModel(IConfiguration config, AppDbContext db, UserManager<AppUser> um, IFileService fs)
        {
            _config = config;
            _db = db;
            _um = um;
            _fs = fs;
        }
        public void OnGet()
        {
            SuccessShow = false;
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (UploadFile != null)
            {
                if (!_fs.ValidateFile(UploadFile))
                {
                    ModelState.TryAddModelError("UploadFile_Type", "The file is not of an allowed type.");
                }

                // Is image less the 2MB?
                // TODO: Parameterized size
                if (UploadFile.Length > 2097152)
                {
                    ModelState.TryAddModelError("UploadFile_Size", "The file size is too big.");
                }
            }
            else
            {
                ModelState.TryAddModelError("UploadFIle_Empty", "There is no file selected for upload.");
            }

            if (!ModelState.IsValid)
                return Page();

            // Save uploaded file
            using (var ms = new MemoryStream())
            {
                await UploadFile.CopyToAsync(ms);

                // Is file less then 2 MB?
                // TODO: Parameterize length. in AppSettings.json or configurable through databse?
                // IS this check really necessary if we check the length above?
                if (ms.Length <= 2097152)
                {
                    await _fs.SaveFile(ms.ToArray(), UploadFile.FileName);
                }
            }

            SuccessShow = true;
            SuccessMessage = "File uploaded successfully!";

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            return await Task.Run(() =>
            {
                if (SelectedFile == null)
                {
                    ModelState.TryAddModelError("SelectedFile", "Missing path for a file to delete.");
                }

                if (!ModelState.IsValid)
                    return Page();

                FileInfo file = new FileInfo(_fs.GetAbsolutePath(SelectedFile));
                file.Delete();

                return Page();
            });
        }
    }
}