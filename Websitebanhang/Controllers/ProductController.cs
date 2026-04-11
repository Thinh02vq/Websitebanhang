using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Repository;

namespace Websitebanhang.Controllers 
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult product()
        {
            return View();
        }
        public async Task<IActionResult> details(int? Id)
        {
            if (Id == null) return RedirectToAction("product");
            var productById = _dataContext.Products.Where(p => p.Id == Id).FirstOrDefault();
            return View(productById);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            // Tìm kiếm sản phẩm theo tên hoặc mô tả
            var products = await _dataContext.Products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();
            ViewBag.Keyword = searchTerm;
            return View(products);
        }
    }
}
