using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using lrndrpub.Models;

namespace lrndrpub.Pages.Admin
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _sm;

        public LogoutModel(SignInManager<AppUser> signInManager)
        {
            _sm = signInManager;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if(User.Identity.IsAuthenticated)
                await _sm.SignOutAsync();

            return RedirectToLocal(returnUrl);
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