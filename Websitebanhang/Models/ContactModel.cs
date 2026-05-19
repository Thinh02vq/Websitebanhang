using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Websitebanhang.Repository.Validation;

namespace Websitebanhang.Models
{
    public class ContactModel
    {
        [Key] 

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập bản đồ")]
        public string Map { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } =  string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập thông tin liên hệ")]
        public string Description { get; set; } = string.Empty;

        public string? LogoImg { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
