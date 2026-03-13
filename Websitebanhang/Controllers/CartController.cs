using Microsoft.AspNetCore.Mvc;

namespace Websitebanhang.Controllers
{
    public class CartController : Controller
    {
        public IActionResult cart()
        {
            return View();
        }
        public IActionResult checkout()
        {
            return View("~/Views/Checkout/checkout.cshtml");
        }
    }
}
