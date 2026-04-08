using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<OrderModel> orders = _dataContext.Orders.ToList();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = orders.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);// 
        }
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var DetailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
            return View(DetailsOrder);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("Admin/Order/UpdateOrder")]
        // Thêm [FromForm] để ép nó đọc dữ liệu từ body của Ajax gửi lên
        public async Task<IActionResult> UpdateOrder([FromForm] string orderCode, [FromForm] int status)
        {
            // Ép kiểu hoặc kiểm tra orderCode có bị null không
            if (string.IsNullOrEmpty(orderCode)) return BadRequest("Mã đơn hàng không hợp lệ");

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null) return NotFound();

            order.status = status;
            await _dataContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật trạng thái thành công" });
        }

        [Route("Admin/Order/Delete")]
        public async Task<IActionResult> Delete(string orderCode)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return NotFound();
            }
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đơn hàng đã được xóa thành công.";
            return RedirectToAction("Index");
        }
    }
}
