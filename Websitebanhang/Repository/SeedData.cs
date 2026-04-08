using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;

namespace Websitebanhang.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                // 1. Tạo Danh mục (Categories)
                CategoryModel iphone = new CategoryModel { Name = "iPhone", Slug = "iphone", Description = "Các dòng điện thoại Apple", Status = 1 };
                CategoryModel samsung = new CategoryModel { Name = "Samsung", Slug = "samsung", Description = "Các dòng điện thoại Samsung", Status = 1 };
                CategoryModel macbook = new CategoryModel { Name = "Macbook", Slug = "macbook", Description = "Laptop của Apple", Status = 1 };
                CategoryModel pc = new CategoryModel { Name = "PC Gaming", Slug = "pc-gaming", Description = "Máy tính chơi game", Status = 1 };

                // 2. Tạo Thương hiệu (Brands)
                BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Thương hiệu Apple", Status = 1 };
                BrandModel ss = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Thương hiệu Samsung", Status = 1 };
                BrandModel dell = new BrandModel { Name = "Dell", Slug = "dell", Description = "Thương hiệu Dell", Status = 1 };

                // 3. Thêm danh sách sản phẩm mẫu
                _context.Products.AddRange(
                    // Nhóm iPhone
                    new ProductModel { Name = "iPhone 15 Pro Max", Slug = "iphone-15-promax", Description = "Chip A17 Pro siêu mạnh", Image = "iphone15.jpg", Category = iphone, Brand = apple, Price = 34000000 },
                    new ProductModel { Name = "iPhone 13", Slug = "iphone-13", Description = "Điện thoại quốc dân", Image = "iphone13.jpg", Category = iphone, Brand = apple, Price = 15000000 },

                    // Nhóm Samsung
                    new ProductModel { Name = "Galaxy S24 Ultra", Slug = "galaxy-s24-ultra", Description = "AI Phone thế hệ mới", Image = "s24.jpg", Category = samsung, Brand = ss, Price = 31000000 },
                    new ProductModel { Name = "Galaxy Z Fold5", Slug = "galaxy-z-fold5", Description = "Điện thoại màn hình gập", Image = "zfold5.jpg", Category = samsung, Brand = ss, Price = 35000000 },

                    // Nhóm Laptop/PC
                    new ProductModel { Name = "Macbook Pro M3", Slug = "macbook-pro-m3", Description = "Sức mạnh từ chip M3", Image = "macm3.jpg", Category = macbook, Brand = apple, Price = 45000000 },
                    new ProductModel { Name = "Dell Alienware M16", Slug = "dell-alienware-m16", Description = "Laptop gaming đỉnh cao", Image = "alienware.jpg", Category = pc, Brand = dell, Price = 55000000 }
                );

                _context.SaveChanges();
            }
        }
        public static async Task SeedUsers(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Danh sách Role cần tạo
            string[] roleNames = { "Admin", "Khách hàng" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Khởi tạo đối tượng Role
                    var role = new IdentityRole(roleName);

                    // TỰ GÁN: Đảm bảo không bị null trong DB
                    role.NormalizedName = roleName.ToUpper();
                    role.ConcurrencyStamp = Guid.NewGuid().ToString();

                    await roleManager.CreateAsync(role);
                }
            }

            // 2. Tạo Admin mặc định
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new AppUserModel
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    // Các trường kỹ thuật quan trọng của User
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // 3. Tạo Khách hàng mặc định
            var customerEmail = "customer@gmail.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);

            if (customerUser == null)
            {
                var newCustomer = new AppUserModel
                {
                    UserName = "khachhang1",
                    Email = customerEmail,
                    EmailConfirmed = true,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(newCustomer, "Pass123@");
                if (result.Succeeded)
                {
                    // Lưu ý: Tên Role gán ở đây phải khớp 100% với tên tạo ở Bước 1
                    await userManager.AddToRoleAsync(newCustomer, "Khách hàng");
                }
            }
        }
    }
}
