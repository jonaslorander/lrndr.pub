using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using lrndrpub.Data;
using lrndrpub.Models;

using Slugify;

namespace lrndrpub.Pages.Admin
{
    public class RegisterModel : PageModel
    {
        #region Properties

        [BindProperty]
        public string FullName { get; set; }

        [Required]
        [BindProperty]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [BindProperty]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[Compare("Password", "Passwords do not match.")]
        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public bool SetAsAdmin { get; set; }

        #endregion

        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _um;

        public bool IsAdmin { get; set; }

        public RegisterModel(AppDbContext db, UserManager<AppUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var author = await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name);
            var IsAdmin = author.IsAdmin;

            if (!IsAdmin)
                return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // register new app user account
            var result = await _um.CreateAsync(new AppUser { UserName = UserName, Email = Email }, Password);

            // add user as author to app database
            var uc = await _db.Authors.CountAsync(a => a.UserName == UserName);
            if (uc == 0 && result.Succeeded)
            {
                var user = new Author
                {
                    Fullname = FullName,
                    Email = Email,
                    //IdentityId = _db.Users.Single(u => u.UserName == UserName).Id,
                    IdentityId = await _um.GetUserIdAsync(await _um.FindByEmailAsync(Email)),
                    UserName = UserName,
                    Slug = await CreateSlug(UserName),
                    IsAdmin = SetAsAdmin,
                    CreatedAt = DateTime.Now,
                    CreatedBy = (await _db.Authors.SingleAsync(a => a.UserName == User.Identity.Name)).AuthorId
                };

                await _db.Authors.AddAsync(user);
                await _db.SaveChangesAsync();

                return RedirectToPage("Users");
            }
            else
            {
                if( uc != 0 )
                    ModelState.AddModelError("Custom", "A user with that username already exists.");
                else
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
            }

            return Page();
        }
        private async Task<string> CreateSlug(string username)
        {
            string slug;
            SlugHelper helper = new SlugHelper();
            slug = helper.GenerateSlug(username);

            return await Task.FromResult(slug);
        }
    }
}