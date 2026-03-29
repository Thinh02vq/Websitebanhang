using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);// Tạo SelectList cho Categories, sử dụng Id làm giá trị và Name làm hiển thị, và chọn CategoryId hiện tại
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);//  Tạo SelectList cho Brands, sử dụng Id làm giá trị và Name làm hiển thị, và chọn BrandId hiện tại

            if (ModelState.IsValid)
            {
                //code them dư lieu
                product.Slug = product.Name.Replace(" ", "-");// Tạo slug từ tên sản phẩm
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);// Kiểm tra xem slug đã tồn tại trong cơ sở dữ liệu hay chưa
                if (slug != null)
                {
                    ModelState.AddModelError("", "Tên sản phẩm đã tồn tại!");
                    return View(product);
                }
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");// Xác định thư mục lưu trữ ảnh
                    string imageName = Guid.NewGuid().ToString() + "-" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _dataContext.Products.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Model có một vài thứ đang bị lỗi!";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return View(product);
            }

            
        }
        
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel? product = await _dataContext.Products.FindAsync(Id);
            if(product == null) return NotFound();
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( ProductModel product)
        {
            if (product== null) return NotFound();
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            var existed_product = _dataContext.Products.Find(product.Id);// Tìm sản phẩm hiện tại trong cơ sở dữ liệu
            if (existed_product == null) return NotFound();
            if (ModelState.IsValid)
            {
                
                product.Slug = product.Name.Replace(" ", "-");
                
                //upload ảnh mới
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "-" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    // Xóa ảnh cũ   
                    if (!string.IsNullOrEmpty(existed_product.Image) && existed_product.Image != "noimage.png")
                    {
                        string oldfilePath = Path.Combine(uploadsDir, existed_product.Image);
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
                    }
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }
                    existed_product.Image = imageName;
                }
                // Cập nhật các thuộc tính của sản phẩm hiện tại với giá trị mới từ form
                existed_product.Name = product.Name;
                existed_product.Slug = product.Slug;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.BrandId = product.BrandId;
                existed_product.CategoryId = product.CategoryId;

                _dataContext.Products.Update(existed_product);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Model có một vài thứ đang bị lỗi!";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return View(product);
            }

            
        }
       
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel? product = await _dataContext.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }
           
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            
                string oldfilePath = Path.Combine(uploadsDir, product.Image!);
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
            
                _dataContext.Products.Remove(product);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Xóa sản phẩm thành công!";
                return RedirectToAction("Index");
        }
        
    }
}
