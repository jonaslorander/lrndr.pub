using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using lrndrpub.Models;

namespace lrndrpub.Data
{
    public class AppDbContext : IdentityDbContext<lrndrpub.Models.AppUser>
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<SettingsValue> Settings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            base.OnModelCreating(modelBuilder);
        }
    }
}
