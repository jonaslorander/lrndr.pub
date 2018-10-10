using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using lrndrpub.Models;

namespace lrndrpub.Data
{
    public class EFConfigurationProvider : ConfigurationProvider
    {
        public EFConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        Action<DbContextOptionsBuilder> OptionsAction { get; }

        // Load config data from EF DB.
        public override void Load()
        {
            Data.Clear();

            var builder = new DbContextOptionsBuilder<AppDbContext>();

            OptionsAction(builder);

            using (var dbContext = new AppDbContext(builder.Options))
            {
                Data = dbContext.Settings.ToDictionary(c => c.Key, c => c.Value);
            }
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
            Data[key] = value;

            var builder = new DbContextOptionsBuilder<AppDbContext>();

            OptionsAction(builder);

            using (var dbContext = new AppDbContext(builder.Options))
            {
                var sv = dbContext.Settings.FirstOrDefault(s => s.Key == key);
                sv.UpdatedAt = DateTime.Now;
                // TODO: sv.UpdatedBy = _db.Authors.Single(a => a.UserName == User.Identity.Name).AuthorId // How to get User here?
                sv.Value = value;
                dbContext.SaveChanges();
            }
        }
    }
}
