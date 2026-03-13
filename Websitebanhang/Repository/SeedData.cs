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
                CategoryModel ịphone = new CategoryModel { Name = "iPhone 14 Pro Max", Slug = "iphone", Description = "Sản phẩm của Apple", Status = 1 };
                CategoryModel sam = new CategoryModel { Name = "Galaxy S23 Ultra", Slug = "sam", Description = "Sản phẩm của SamSung", Status = 1 };

                BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Sản phẩm của Apple",  Status = 1 };
                BrandModel samsung = new BrandModel { Name = "SamSung", Slug = "samsung", Description = "Sản phẩm của SamSung",  Status = 1 };

                _context.Products.AddRange(
                    new ProductModel { Name = "iPhone 14 Pro Max", Slug = "iphone", Description = "Điện thoại cao cấp của Apple", Image= "1.jpg", Category = ịphone, Brand = apple, Price = 30000000 },
                    new ProductModel { Name = "Galaxy S23 Ultra", Slug = "sam", Description = "Điện thoại cao cấp của SamSung", Image = "1.jpg", Category = sam, Brand = samsung, Price = 30000000}
                );
                _context.SaveChanges();
            }

        }
    }
}
