using Microsoft.AspNetCore.Mvc;
using Websitebanhang.Models;
using Websitebanhang.Models.ViewModel;
using Websitebanhang.Repository;

namespace Websitebanhang.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }

        public IActionResult cart()
        {
            List<CartModel> cartItems = HttpContext.Session.getJson<List<CartModel>>("Cart") ?? new List<CartModel>();  
            CartViewModel cartViewModel = new CartViewModel
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartViewModel);
        }

        public IActionResult checkout()
        {
            return View("~/Views/Checkout/checkout.cshtml");
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductModel? product = await _dataContext.Products.FindAsync(Id);
            if (product == null) return NotFound();
            List<CartModel> cart = HttpContext.Session.getJson<List<CartModel>>("Cart") ?? new List<CartModel>();
            CartModel? cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItems == null)
            {
                cart.Add(new CartModel(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.setJson("Cart", cart);

            TempData["Success"] = "Thêm sản phẩm vào giỏ hàng thành công!";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartModel> cart = HttpContext.Session.getJson<List<CartModel>>("Cart") ?? new List<CartModel>();
            if (cart == null) return RedirectToAction("Index");
            CartModel? cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItems == null) return RedirectToAction("Index");

            if (cartItems.Quantity > 1)
            {
                --cartItems.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.setJson("Cart", cart);
            }
            return RedirectToAction("cart");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            List<CartModel> cart = HttpContext.Session.getJson<List<CartModel>>("Cart") ?? new List<CartModel>();
            if (cart == null)
            {
                return RedirectToAction("Index"); 
            }
            CartModel? cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItems == null)
            {
                return RedirectToAction("Index");
            }
            if (cartItems.Quantity >= 1)
            {
                ++cartItems.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.setJson("Cart", cart);
            }
            return RedirectToAction("cart");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartModel> cart = HttpContext.Session.getJson<List<CartModel>>("Cart") ??new List<CartModel>();
            cart.RemoveAll(p => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.setJson("Cart", cart);
            }
            return RedirectToAction("cart");
        }
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Đã xoá các sản phẩm trong giỏ hàng!";
            return RedirectToAction("cart");
        }
    }
}
