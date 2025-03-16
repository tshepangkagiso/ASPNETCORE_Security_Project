using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_Identity.Data.Account;

namespace WebApp_Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        [BindProperty]
        public string Message { get; set; } = string.Empty;

        private readonly UserManager<User> userManager;
        public ConfirmEmailModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await this.userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    this.Message = "Email address is successfully confirmed, you may try to login";
                    return Page();                   
                    //return RedirectToPage("/Account/Login");
                }
            }
            this.Message = "Failed to validate email";
            return Page();

        }
    }
}
