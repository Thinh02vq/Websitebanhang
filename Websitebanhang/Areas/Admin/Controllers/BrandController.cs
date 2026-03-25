using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> Create ()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel? brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);// Kiểm tra xem slug đã tồn tại trong cơ sở dữ liệu hay chưa
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại!");
                    return View(brand);
                }
                _dataContext.Brands.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Thêm thương hiệu thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Thêm thương hiệu thất bại!";
                List<string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return View(brand);
            } 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại!");
                    return View(brand);
                }
                _dataContext.Brands.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Sửa danh mục thất bại!";
                List<string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return View(brand);
            }

        }
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel? brand = await _dataContext.Brands.FindAsync(Id);
            if (brand == null) return NotFound();
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["Success"] = "Xóa thương hiệu thành công!";
            return RedirectToAction("Index");
        }
    }
}
