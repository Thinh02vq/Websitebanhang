using Microsoft.AspNetCore.Identity;

namespace Websitebanhang.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; } = string.Empty;
    }
}
