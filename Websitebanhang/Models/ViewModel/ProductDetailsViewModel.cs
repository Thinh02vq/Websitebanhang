using System.ComponentModel.DataAnnotations;
using Websitebanhang.Models;
namespace Websitebanhang.Models.ViewModel
{
    public class ProductDetailsViewModel
    {
        public ProductModel? ProductDetails { get; set; }

        public IEnumerable<RaitingModel> Raitings{ get; set; } = Enumerable.Empty<RaitingModel>();

        [Required(ErrorMessage = "Yêu cầu nhập đánh giá")]
        public string Comments { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;


    }
}
