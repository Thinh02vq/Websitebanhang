namespace Websitebanhang.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } 
        
        public int status { get; set; } 
    }
}
