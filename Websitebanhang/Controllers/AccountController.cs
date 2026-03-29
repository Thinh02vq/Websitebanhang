using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Websitebanhang.Models;
using Websitebanhang.Models.ViewModel;

namespace Websitebanhang.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return LocalRedirect(string.IsNullOrEmpty(loginVM.ReturnUrl) ? "/" : loginVM.ReturnUrl);
                }
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không hợp lệ vui lòng kiểm tra lại thông tin!");
            }
            return View(loginVM);
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(UserModel user)
        {
            if (ModelState.IsValid)
            {
                // Tạo một đối tượng user mới từ thông tin người dùng
                AppUserModel newUser = new AppUserModel
                {
                    UserName = user.UserName,
                    Email = user.Email
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password); // Tạo người dùng mới
                if (result.Succeeded)
                {
                    // Nếu tạo người dùng thành công, đăng nhập người dùng
                    TempData["success"] = "Tạo tài khoản thành công";
                    return RedirectToAction("Login", "Account");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); // Thêm lỗi vào ModelState để hiển thị trên view
                }
            }
            return View(user);
        }
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Đăng xuất người dùng
            return RedirectToAction("Index", "Home");
        }
    }
}
