using Microsoft.AspNetCore.Identity;

namespace WebApp_Identity.Data.Account
{
    public class User : IdentityUser
    {
        // we are changing User by adding new fields to the already existing fields. We are not removing existing fields.
        public string Department { get; set; } = string.Empty;
        public string Postion { get; set; } = string.Empty;
    }
}
