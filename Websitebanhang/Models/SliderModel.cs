using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Websitebanhang.Repository.Validation;

namespace Websitebanhang.Models
{
    public class SliderModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống tên slider")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Không được để trống mô tả slider")]
        public string Description { get; set; } = string.Empty;

        public int? Status { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
