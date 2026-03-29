using System.ComponentModel.DataAnnotations;

namespace Websitebanhang.Models.ViewModel
{
    public class LoginViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password), Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        public string Password { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;
    }
}
