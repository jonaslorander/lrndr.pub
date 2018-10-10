using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using lrndrpub.Models;

namespace lrndrpub.Pages.Admin
{
    public class LoginModel : PageModel
    {
        [Required]
        [BindProperty]
        public string UserName { get; set; }

        [Required]
        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        private readonly SignInManager<AppUser> _sm;

        public LoginModel(SignInManager<AppUser> sm)
        {
            _sm = sm;
        }

        public void OnGet(string ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;

            if(ModelState.IsValid)
            {
                var res = await _sm.PasswordSignInAsync(UserName, Password, RememberMe, false);
                if(res.Succeeded)
                {
                    return RedirectToLocal(ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }
    }
}