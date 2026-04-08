using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUserModel> _userManager;
        public RoleController(DataContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<IdentityRole> roles = _dataContext.Roles.ToList();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = roles.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = roles.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);// 
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Admin/Role/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra xem tên Role đã tồn tại chưa (Dùng RoleManager cho đồng bộ)
                var roleExist = await _roleManager.RoleExistsAsync(role.Name!);

                if (roleExist)
                {
                    ModelState.AddModelError("", "Role này đã tồn tại trong hệ thống!");
                    return View(role);
                }

                // 2. Tạo đối tượng Role mới
                var result = await _roleManager.CreateAsync(new IdentityRole(role.Name!));

                if (result.Succeeded)
                {
                    TempData["Success"] = "Thêm role mới thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Nếu có lỗi từ phía Identity (ví dụ tên role chứa ký tự lạ)
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // Nếu ModelState không hợp lệ hoặc có lỗi xảy ra
            TempData["Error"] = "Vui lòng kiểm tra lại thông tin nhập vào!";
            return View(role);
        }

        [Route("Admin/Role/Delete/{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(Id);
            if (roleToDelete == null) return NotFound();

            // 1. Tìm tất cả người dùng thuộc Role này
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleToDelete.Name!);

            // 2. Nếu có người dùng, hãy chuyển họ sang Role khác (ví dụ: "Khách hàng")
            if (usersInRole.Any())
            {
                // Kiểm tra xem Role dự phòng có tồn tại không
                var defaultRole = "Khách hàng";
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    TempData["Error"] = $"Không thể xóa vì còn người dùng, và Role '{defaultRole}' để thay thế chưa được tạo!";
                    return RedirectToAction("Index");
                }

                foreach (var user in usersInRole)
                {
                    // Xóa quyền cũ
                    await _userManager.RemoveFromRoleAsync(user, roleToDelete.Name!);
                    // Thêm quyền mới
                    await _userManager.AddToRoleAsync(user, defaultRole);
                }
            }

            // 3. Sau khi đã "dọn sạch" User khỏi Role, tiến hành xóa Role
            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Đã xóa Role '{roleToDelete.Name}' và chuyển {usersInRole.Count} người dùng sang Role mặc định.";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa Role.";
            }

            return RedirectToAction("Index");
        }
    }
}
