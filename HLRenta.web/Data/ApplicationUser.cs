using Microsoft.AspNetCore.Identity;

namespace HLRenta.web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string name { get; set; }
    }

}
