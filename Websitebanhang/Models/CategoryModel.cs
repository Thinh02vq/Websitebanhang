using System.ComponentModel.DataAnnotations;

namespace Websitebanhang.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Yêu cầu nhập tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập mô tả danh mục")]
        public string Description { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public int Status { get; set; }


    }
}
