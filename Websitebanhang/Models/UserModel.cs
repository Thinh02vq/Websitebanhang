using System.ComponentModel.DataAnnotations;

namespace Websitebanhang.Models
{
    public class UserModel
    {
        public string Id { get; set; }  = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập email"),EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại"), Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [DataType(DataType.Password),Required(ErrorMessage ="Yêu cầu nhập mật khẩu")]
        public string Password { get; set; } = string.Empty;    
    }
}
