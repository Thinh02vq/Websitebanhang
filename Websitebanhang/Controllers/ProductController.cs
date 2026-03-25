using Microsoft.AspNetCore.Mvc;
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
    }
}
