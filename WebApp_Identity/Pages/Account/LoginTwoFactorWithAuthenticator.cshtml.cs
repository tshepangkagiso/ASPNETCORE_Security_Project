using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp_Identity.Data.Account;

namespace WebApp_Identity.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public AuthenticatorViewModel AuthenticatorMFA { get; set; }

        public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
        {
            this.AuthenticatorMFA = new AuthenticatorViewModel();
            this.signInManager = signInManager;
        }
        public void OnGet(bool rememberMe)
        {
            this.AuthenticatorMFA.SecurityCode = string.Empty;
            this.AuthenticatorMFA.RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await this.signInManager.TwoFactorAuthenticatorSignInAsync(this.AuthenticatorMFA.SecurityCode, this.AuthenticatorMFA.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("LoginMFA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("LoginMFA", "Failed to login in, please try again.");
                }
                return Page();
            }
        }
    }

    public class AuthenticatorViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
