using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace lrndrpub.Services
{
    public interface IFileService
    {
        bool ValidateFile(IFormFile file);
        bool ValidateImageFile(IFormFile file);
        bool ValidateImageFile(string ext);

        Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null);

        List<FileInfo> GetImages(short year = 0, short month = 0);
        List<FileInfo> GetFiles(short year = 0, short month = 0);

        string GetWebPath(string path);
        string GetAbsolutePath(string webpath);
    }
}
