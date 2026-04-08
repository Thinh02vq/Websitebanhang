using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Websitebanhang.Areas.Admin.Repository;
using Websitebanhang.Models;
using Websitebanhang.Models.ViewModel;

namespace Websitebanhang.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
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
                    TempData["success"] = "Đăng nhập thành công";
                    
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
                AppUserModel newUser = new AppUserModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    // 3. TỰ ĐỘNG GÁN ROLE "Khách hàng"
                    // Kiểm tra xem Role "Khách hàng" đã tồn tại trong DB chưa
                    if (await _roleManager.RoleExistsAsync("Khách hàng"))
                    {
                        await _userManager.AddToRoleAsync(newUser, "Khách hàng");
                    }
                    else
                    {
                        // Nếu chưa có (đề phòng SeedData chưa chạy), bạn có thể tạo mới luôn hoặc log lỗi
                        await _roleManager.CreateAsync(new IdentityRole("Khách hàng"));
                        await _userManager.AddToRoleAsync(newUser, "Khách hàng");
                    }

                    TempData["success"] = "Tạo tài khoản thành công với quyền Khách hàng";
                    return RedirectToAction("Login", "Account");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
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
