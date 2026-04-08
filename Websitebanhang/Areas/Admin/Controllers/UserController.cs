using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = context;
        }
        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Sử dụng LINQ để join bảng Users, UserRoles và Roles để lấy thông tin user cùng với tên role
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId into urGroup
                                        from ur in urGroup.DefaultIfEmpty()
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id into rGroup
                                        from r in rGroup.DefaultIfEmpty()
                                        select new AppUserModel // Tạo một đối tượng AppUserModel mới để chứa thông tin user và role
                                        {
                                            Id = u.Id,
                                            UserName = u.UserName,
                                            Email = u.Email,
                                            PasswordHash = u.PasswordHash,
                                            PhoneNumber = u.PhoneNumber,
                                            RoleName = r.Name ?? "No Role" 
                                        }).ToListAsync();

            return View(usersWithRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }

        [Route("Admin/User/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash!);// Sử dụng PasswordHash để lưu mật khẩu tạm thời, sau đó sẽ được mã hóa bởi UserManager
                if (createUserResult.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(user.Email!);// Tìm user vừa tạo theo email
                    var userId = createdUser?.Id;// Lấy Id của user vừa tạo
                    var role = await _roleManager.FindByIdAsync(user.RoleId);// Tìm role theo RoleId

                    var addToRoleResult = await _userManager.AddToRoleAsync(createdUser!, role!.Name!);// Thêm user vào role
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }
            }
            else
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n ", errors);
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [Route("Admin/User/Edit/{Id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(Id);
            if (existingUser == null) return NotFound();

            if (ModelState.IsValid)
            {
                // 1. Cập nhật thông tin cơ bản
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

                var updateUserResult = await _userManager.UpdateAsync(existingUser);

                if (updateUserResult.Succeeded)
                {
                    // 2. XỬ LÝ ROLE (Quan trọng nhất ở đây)

                    // Lấy danh sách Role hiện tại của User (ví dụ đang là "Sales")
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);

                    // Tìm thông tin Role mới từ RoleId mà bạn chọn ở Dropdown (ví dụ "Khách hàng")
                    var newRole = await _roleManager.FindByIdAsync(user.RoleId);

                    if (newRole != null)
                    {
                        // Bước A: Xóa toàn bộ Role cũ của User này
                        await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);

                        // Bước B: Thêm Role mới vào cho User
                        await _userManager.AddToRoleAsync(existingUser, newRole.Name!);
                    }

                    TempData["Success"] = "Cập nhật User và Quyền thành công!";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(updateUserResult);
                }
            }

            // Nếu có lỗi, load lại Roles cho View
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(existingUser);
        }

        [Route("Admin/User/Delete/{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteUserResult = await _userManager.DeleteAsync(user);
            if (deleteUserResult.Succeeded)
            {
                TempData["Success"] = "Xóa User thành công.";
                return RedirectToAction("Index", "User");
            }
            else
            {
                AddIdentityErrors(deleteUserResult);
                return View("Error");
            }
        }
    }
}
