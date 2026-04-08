using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Controllers 
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext context, IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);// Lấy email của người dùng đã đăng nhập
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else 
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItems = new OrderModel();
                orderItems.OrderCode = ordercode;
                orderItems.UserName = userEmail;
                orderItems.status = 1;
                orderItems.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItems);
                _dataContext.SaveChanges();
                List<CartModel> cartItems = HttpContext.Session.getJson<List<CartModel>>("Cart") ?? new List<CartModel>();
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetails();
                    orderDetail.UserName = userEmail;
                    orderDetail.OrderCode = ordercode;
                    orderDetail.ProductId = item.ProductId;
                    orderDetail.Price = item.Price;
                    orderDetail.Quantity = item.Quantity;
                    _dataContext.Add(orderDetail);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                // Gửi email thông báo cho người dùng
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đơn hàng của bạn đã được đặt thành công!";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["Success"] = "Đặt hàng thành công chờ duyệt đơn hàng!";
                return RedirectToAction("cart", "Cart");
            }
            
        }
    }
}
