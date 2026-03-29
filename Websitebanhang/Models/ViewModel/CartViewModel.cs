namespace Websitebanhang.Models.ViewModel
{
    public class CartViewModel
    {
        public List<CartModel> CartItems { get; set; } = new List<CartModel>();

        public decimal GrandTotal { get; set; }


    }
}
