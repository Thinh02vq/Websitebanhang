using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Websitebanhang.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; } = string.Empty;
        
        public string RoleId { get; set; } = string.Empty;

        [NotMapped]
        public string RoleName { get; set; } = "No Role";
    }
}
