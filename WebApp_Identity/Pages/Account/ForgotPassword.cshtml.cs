using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp_Identity.Data.Account;
using WebApp_Identity.Services;

namespace WebApp_Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        

        [BindProperty]
        [Display(Name = "Enter your email")]
        public string Email { get; set; } = string.Empty;
        public string? SuccessMessage {  get; set; }
        public IEmailService EmailService { get; }

        private readonly UserManager<User> userManager;

        public ForgotPasswordModel(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            EmailService = emailService;
        }
        public void OnGet()
        {
            this.SuccessMessage = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            //get user
            var user = await this.userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                //generate password reset token
                var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

                //send email with link to recover password page with query string
                // email dry run
                return Redirect(Url.PageLink(pageName:"/Account/RecoverPassword", values: new {userId = user.Id, token = token})??"");

                //Actual sending of email
                /*var confirmationLink = Url.PageLink(pageName:"/Account/RecoverPassword", values: new {userId = user.Id, token = token})??"";
                var emailSender = "tshepangkagisomashigo8@outlook.com";
                var sendEmailTo = user.Email??"";
                var emailSubject = "";
                var emailBody = "";
                await EmailService.SendAsync(emailSender, sendEmailTo ,emailSubject,emailBody);
                this.SuccessMessage = "Email has been sent successfully, check your email.";
                return Page();*/
            }
            ModelState.AddModelError("ForgetPassword", "Failed to send reset email ");
            return Page();

        }
    }
}
