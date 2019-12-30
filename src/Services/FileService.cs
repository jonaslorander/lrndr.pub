using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using lrndrpub.Data;
using lrndrpub.Models;

namespace lrndrpub.Services
{
    public class FileService : IFileService
    {
        private readonly string[] _allowedOtherFileTypes = { ".doc", ".docx", ".pdf", ".zip", ".xls", ".xlsx", ".txt" };
        private readonly string[] _allowedImageFileTypes = { ".png", ".jpg", ".jpeg", ".gif", ".svg" };
        private readonly string[] _allowedFileTypes;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        private readonly string _folder;

        public FileService(IWebHostEnvironment env, IHttpContextAccessor contextAccessor, IConfiguration config, AppDbContext db)
        {
            _contextAccessor = contextAccessor;
            _config = config;
            _db = db;

            _folder = env.WebRootPath;

            _allowedFileTypes = new string[_allowedImageFileTypes.Length + _allowedOtherFileTypes.Length];
            Array.Copy(_allowedImageFileTypes, _allowedFileTypes, _allowedImageFileTypes.Length);
            Array.Copy(_allowedOtherFileTypes, 0, _allowedFileTypes, _allowedImageFileTypes.Length, _allowedOtherFileTypes.Length);
        }

        public bool ValidateFile(IFormFile file)
        {
            return _allowedFileTypes.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase);
        }

        public bool ValidateImageFile(IFormFile file)
        {
            return ValidateImageFile(Path.GetExtension(file.FileName));
        }

        public bool ValidateImageFile(string ext)
        {
            return _allowedImageFileTypes.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

        /*
         * Saves a file on disk. The folder structure will be:
         * /wwwroot/images/YEAR/MONTH/file.ext for images
         * /wwwroot/files/YEAR/MONTH/file.ext for all other files
         */
        public async Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null)
        {
            string subpath = string.Empty;

            suffix = CleanFromInvalidChars(suffix ?? DateTime.UtcNow.Ticks.ToString());

            string ext = Path.GetExtension(fileName);
            string name = CleanFromInvalidChars(Path.GetFileNameWithoutExtension(fileName));

            if (_allowedImageFileTypes.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                subpath = "images";
            }
            else if(_allowedFileTypes.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                subpath = "files";
            }
            else
            {
                // Throw excption if filestype is not allowed. 
                throw new Exception($"File type {ext} is not allowed.");
            }

            string fileNameWithSuffix = $"{name}_{suffix}{ext}";
            string webpath = Path.Combine(new string[] {
                subpath,
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                fileNameWithSuffix
            });

            string absolute = Path.Combine(_folder, webpath);
            string dir = Path.GetDirectoryName(absolute);

            Directory.CreateDirectory(dir);
            using (var writer = new FileStream(absolute, FileMode.CreateNew))
            {
                await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }

            return $"/{webpath.Replace('\\', '/')}";
        }

        private static string CleanFromInvalidChars(string input)
        {
            // ToDo: what we are doing here if we switch the blog from windows
            // to unix system or vice versa? we should remove all invalid chars for both systems

            var regexSearch = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
            var r = new Regex($"[{regexSearch}]");
            return r.Replace(input, "");
        }

        /*
         * Returns a list of all images storled in wwwroot/images
         * if year is left out all images will be returned
         * year must be supplied if month is supplied
         * 
         */
        public List<FileInfo> GetImages(short year = 0, short month = 0)
        {
            if (month != 0 && year == 0)
                throw new DirectoryNotFoundException("Year must be supplied as argument when supplying month as argument.");

            // Setup search path
            string _searchFolder = Path.Combine(_folder, "images");

            if (month >= 1 && month <= 12 && year > 0)
            {
                _searchFolder = Path.Combine(_searchFolder, year.ToString(), month.ToString());
            }
            else if (year > 0)
            {
                _searchFolder = Path.Combine(_searchFolder, year.ToString());
            }

            // Search for all image files in specified year and month
            List<FileInfo> files = new List<FileInfo>();

            foreach (string imageFile in Directory.GetFiles(_searchFolder, "*.*", SearchOption.AllDirectories)
                .Where(s => _allowedImageFileTypes.Contains(Path.GetExtension(s), StringComparer.OrdinalIgnoreCase)))
            {
                files.Add(new FileInfo(imageFile));
            }

            return files;
        }

        /*
         * Returns a list of all files storled in wwwroot/files
         * if year is left out all images will be returned
         * year must be supplied if month is supplied
         * 
         */
        public List<FileInfo> GetFiles(short year = 0, short month = 0)
        {
            if (month != 0 && year == 0)
                throw new DirectoryNotFoundException("Year must be supplied as argument when supplying month as argument.");

            // Setup search path
            string _searchFolder = Path.Combine(_folder, "files");

            if (month >= 1 && month <= 12 && year > 0)
            {
                _searchFolder = Path.Combine(_searchFolder, year.ToString(), month.ToString());
            }
            else if (year > 0)
            {
                _searchFolder = Path.Combine(_searchFolder, year.ToString());
            }

            // Search for all image files in specified year and month
            List<FileInfo> files = new List<FileInfo>();

            foreach (string imageFile in Directory.GetFiles(_searchFolder, "*.*", SearchOption.AllDirectories)
                .Where(s => _allowedOtherFileTypes.Contains(Path.GetExtension(s), StringComparer.OrdinalIgnoreCase)))
            {
                files.Add(new FileInfo(imageFile));
            }

            return files;
        }
        public string GetWebPath(string path)
        {
            return (new Uri(path)).ToString().Split("wwwroot", 2)[1];
        }
        public string GetAbsolutePath(string webpath)
        {
            return Path.GetFullPath(_folder + webpath);
        }
    }
}
