using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_Identity.Data.Account;
using WebApp_Identity.Services;

namespace WebApp_Identity.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<User> signInManager;
        const string tokenProvider = "Email";

        [BindProperty]
        public SecurityTokenViewModel SecurityToken { get; set; }

        public LoginTwoFactorModel(UserManager<User> userManager, IEmailService emailService, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.SecurityToken = new SecurityTokenViewModel();
        }
        public async Task OnGetAsync(string email, bool rememberMe)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            
            if(user != null)
            {
                var securityTokenCode = await this.userManager.GenerateTwoFactorTokenAsync(user,tokenProvider);
                SecurityToken.Token = string.Empty;
                SecurityToken.RememberMe = rememberMe;

                string emailSender = "tshepangkagisomashigo8@outlook.com";
                string emailSendTo = user.Email ?? "";
                string emailSubject = "Requested OTP";
                string emailBody = $"Enter this OPT: {securityTokenCode}";

                await this.emailService.SendAsync(emailSender, emailSendTo, emailSubject, emailBody);
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await this.signInManager.TwoFactorSignInAsync(tokenProvider, SecurityToken.Token, SecurityToken.RememberMe,false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to login in, please try again.");
                }
                return Page();
            }
        }
    }

    public class SecurityTokenViewModel
    {
        public string Token { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
