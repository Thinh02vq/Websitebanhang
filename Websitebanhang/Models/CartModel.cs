namespace Websitebanhang.Models
{
    public class CartModel 
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return Quantity * Price;
            }
        }

        public string Image { get; set; } = string.Empty;
        public CartModel()
        {

        }
        public CartModel(ProductModel product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price= product.Price;
            Quantity = 1;
            Image = product.Image ?? "noimage.png"; ;
        }
    }
}
