using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());// Truy vấn tất cả sản phẩm, sắp xếp theo Id giảm dần, và bao gồm thông tin về Category và Brand
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name"); // Tạo SelectList cho Categories, sử dụng Id làm giá trị và Name làm hiển thị
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");// Tạo SelectList cho Brands, sử dụng Id làm giá trị và Name làm hiển thị
            return View();
        }

        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);// Tạo SelectList cho Categories, sử dụng Id làm giá trị và Name làm hiển thị, và chọn CategoryId hiện tại
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);//  Tạo SelectList cho Brands, sử dụng Id làm giá trị và Name làm hiển thị, và chọn BrandId hiện tại
            return View(product);
        }
    }
}
