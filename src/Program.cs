using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using lrndrpub.Data;
using lrndrpub.Models;

namespace lrndrpub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            // Seed database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var um = (UserManager<AppUser>)services.GetRequiredService(typeof(UserManager<AppUser>));
                    if(!um.Users.Any())
                    {
                        um.CreateAsync(new AppUser { UserName = "admin", Email = "admin@example.com" }, "adminpass");
                        um.CreateAsync(new AppUser { UserName = "user", Email = "user@example.com" }, "userpass");
                    }

                    var context = services.GetRequiredService<AppDbContext>();
                    DbInitializer.Initialize(context, um);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
            

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                /*.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })*/
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddAppConfiguration(options => options.UseSqlite(@"DataSource=.\app.db"));
                    })
                    .UseStartup<Startup>();
                });
    }
}
