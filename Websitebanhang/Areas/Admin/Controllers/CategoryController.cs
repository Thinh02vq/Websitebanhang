using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int pg=1)
        {
            List<CategoryModel> category = _dataContext.Categories.ToList();
            const int pageSize = 10;
            if (pg < 1) 
            {
                pg = 1;
            }
            int recsCount = category.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);// 
        }
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel? category = await _dataContext.Categories.FindAsync(Id);
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Admin/Category/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");// Tạo slug từ tên danh mục
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);// Kiểm tra xem slug đã tồn tại trong cơ sở dữ liệu hay chưa
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại!");
                    return View(category);
                }
                _dataContext.Categories.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction("Index");
            }
            else 
            {
                TempData["Error"] = "Thêm danh mục thất bại!";
                List<string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return View(category);
            }
            
        }

        [Route("Admin/Category/Edit/{Id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại!");
                    return View(category);
                }
                _dataContext.Categories.Update(category);
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
                return View(category);
            }
            
        }

        [Route("Admin/Category/Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel? category = await _dataContext.Categories.FindAsync(Id);
            if(category == null) return NotFound();
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
