using System.ComponentModel.DataAnnotations.Schema;

namespace Websitebanhang.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string OrderCode { get; set; } = string.Empty;

        public int ProductId { get; set; } 

        public decimal Price { get; set; }
        
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
    }
}
