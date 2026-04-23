using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
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
            var productById = await _dataContext.Products.Include(p => p.Raiting).FirstOrDefaultAsync(p => p.Id == Id);
            if (productById == null)
            {
                return NotFound(); 
            }
            var relatedProducts = await _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
                .Take(4)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            var ViewModel = new Models.ViewModel.ProductDetailsViewModel
            {
                ProductDetails = productById,
                Raitings = productById.Raiting
            };  
            return View(ViewModel);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            // Tìm kiếm sản phẩm theo tên hoặc mô tả
            var products = await _dataContext.Products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();
            ViewBag.Keyword = searchTerm;
            return View(products);
        }

        public async Task<IActionResult> CommentProduct(RaitingModel raiting)
        {
            if (ModelState.IsValid)
            {
                var raitingEntity = new RaitingModel
                {
                    ProductId = raiting.ProductId,
                    Comments = raiting.Comments,
                    Name = raiting.Name,
                    Email = raiting.Email,
                    Stars = raiting.Stars
                };
                _dataContext.Raitings.Add(raitingEntity);
                await _dataContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi đánh giá. Vui lòng thử lại.";
                List <string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n ", errors);
                return RedirectToAction("details", new { Id = raiting.ProductId});
            }
        }
    }
}
