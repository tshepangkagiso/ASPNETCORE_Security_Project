using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp_Identity.Data.Account;

namespace WebApp_Identity.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();

        public LoginModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            var result = await this.signInManager.PasswordSignInAsync(
                this.Credential.Email, 
                this.Credential.Password, 
                this.Credential.RememberMe,
                false);

            if(result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login in, please try again.");
                }
                    return Page();
            }

        }
    }

    public class CredentialViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
