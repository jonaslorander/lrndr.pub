using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using lrndrpub.Data;
using lrndrpub.Models;

namespace lrndrpub.Services
{
    public class PostService : IPostService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public PostService(IHttpContextAccessor contextAccessor, IConfiguration config, AppDbContext db)
        {
            _contextAccessor = contextAccessor;
            _config = config;
            _db = db;
        }
    }
}
