using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

var builder = WebApplication.CreateBuilder(args);

// connection DB
builder.Services.AddDbContext<DataContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("connectDB")));
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;

    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "category",
    pattern: "/catgory/{Slug?}",
    defaults: new { controller = "Category", action = "category" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "brand" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    try
    { 
        context.Database.Migrate();

        SeedData.SeedingData(context);
        // Sau khi Migrate xong thì gọi Seed Data
        var userManager = services.GetRequiredService<UserManager<AppUserModel>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        
        SeedData.SeedUsers(userManager, roleManager).Wait(); // Seed User/Role
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra trong quá trình Migration dữ liệu.");
    }
}

app.Run();
