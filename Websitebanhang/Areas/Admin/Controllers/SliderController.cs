using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SliderController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Sliders.OrderByDescending(p => p.Id).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        [Route("Admin/Slider/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    // 1. Đảm bảo thư mục tồn tại
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    // 2. Sử dụng using để giải phóng file ngay lập tức
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await slider.ImageUpload.CopyToAsync(fs);
                    }
                    slider.Image = imageName;
                }

                _dataContext.Add(slider);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Thêm mới slider thành công!";
                return RedirectToAction("Index");
            }

            else
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["Error"] = "Lỗi cụ thể: " + message; // Nó sẽ liệt kê chính xác trường nào bị lỗi
                return View(slider);
            }
        }

        public async Task<IActionResult> Edit(int Id)
        {
            SliderModel? slider = await _dataContext.Sliders.FindAsync(Id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        [Route("Admin/Slider/Edit/{Id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderModel slider)
        {
            if (slider == null) return NotFound();
            var existed_slider =  _dataContext.Sliders.Find(slider.Id);// Tìm slider hiện tại trong database
            if (existed_slider == null) return NotFound();
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await slider.ImageUpload.CopyToAsync(fs);
                    }
                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(existed_slider.Image))
                    {
                        string oldfilePath = Path.Combine(uploadsDir, existed_slider.Image);
                        try
                        {
                            if (System.IO.File.Exists(oldfilePath))
                            {
                                System.IO.File.Delete(oldfilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Xóa ảnh cũ thất bại: " + ex.Message);
                            return View(slider);
                        }
                    }
                    existed_slider.Image = imageName;
                }
                existed_slider.Name = slider.Name;
                existed_slider.Description = slider.Description;
                existed_slider.Status = slider.Status;

                _dataContext.Update(existed_slider);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Cập nhật slider thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["Error"] = "Lỗi cụ thể: " + message; // Nó sẽ liệt kê chính xác trường nào bị lỗi
                return View(slider);
            }
        }

        [Route("Admin/Slider/Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            SliderModel? slider = await _dataContext.Sliders.FindAsync(Id);
            if (slider == null)
            {
                return NotFound();
            }

            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");

            string oldfilePath = Path.Combine(uploadsDir, slider.Image!);
            try
            {
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Xóa ảnh cũ thất bại: " + ex.Message);
            }

            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();
            TempData["Success"] = "Xóa slider thành công!";
            return RedirectToAction("Index");
        }
    }
}
