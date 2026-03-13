using Microsoft.EntityFrameworkCore;
using Websitebanhang.Repository;

var builder = WebApplication.CreateBuilder(args);

// connection DB
builder.Services.AddDbContext<DataContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("connectDB")));
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
