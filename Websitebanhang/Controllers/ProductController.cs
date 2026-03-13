using Microsoft.AspNetCore.Mvc;

namespace Websitebanhang.Controllers 
{
    public class ProductController : Controller
    {
        public IActionResult product()
        {
            return View();
        }
        public IActionResult details()
        {
            return View();
        }
    }
}
