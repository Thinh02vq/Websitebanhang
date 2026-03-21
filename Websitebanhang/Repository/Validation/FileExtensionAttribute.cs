using System.ComponentModel.DataAnnotations;

namespace Websitebanhang.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName); // Lấy phần mở rộng của file
                string[] extensions = { ".jpg", ".jpeg", ".png" };// Các phần mở rộng được phép

                bool result = extension.Any(x => extension.EndsWith(x));// Kiểm tra xem phần mở rộng có nằm trong danh sách cho phép hay không
                if (!result)
                {
                    return new ValidationResult("File không hợp lệ. Vui lòng chọn file có định dạng jpg, jpeg hoặc png.");
                }
            } 
            return ValidationResult.Success;
        }
    }
}
