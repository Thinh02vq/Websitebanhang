using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Websitebanhang.Models
{
    public class RaitingModel
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập đánh giá")]
        public string Comments { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập email")]
        public string Email { get; set; } = string.Empty;

        public string Stars { get; set; } = string.Empty;

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
    }
}
