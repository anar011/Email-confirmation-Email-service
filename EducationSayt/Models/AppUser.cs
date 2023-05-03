using Microsoft.AspNetCore.Identity;

namespace EducationSayt.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
