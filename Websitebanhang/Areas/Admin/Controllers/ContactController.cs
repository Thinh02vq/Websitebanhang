using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitebanhang.Models;
using Websitebanhang.Repository;

namespace Websitebanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var contact = _dataContext.Contacts.ToList();
            return View(contact);
        }
        public async Task<IActionResult> Edit()
        {
            ContactModel? contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var existed_contact = _dataContext.Contacts.FirstOrDefault();
            if (existed_contact == null) return NotFound();
            if (ModelState.IsValid)
            {
                if (contact.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                    string imageName = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    if (!string.IsNullOrEmpty(existed_contact.LogoImg) && existed_contact.LogoImg != "noimage.png")
                    {
                        string oldfilePath = Path.Combine(uploadDir, existed_contact.LogoImg);
                        try
                        {
                            if (System.IO.File.Exists(oldfilePath))
                            {
                                System.IO.File.Delete(oldfilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Xóa ảnh cũ thất bại: " + ex.Message);
                        }
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await contact.ImageUpload.CopyToAsync(fileStream);
                    }
                    existed_contact.LogoImg = imageName;
                }

                existed_contact.Name = contact.Name;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Map = contact.Map;
                existed_contact.Email = contact.Email;

                _dataContext.Update(existed_contact);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin liên hệ thành công!";
                return RedirectToAction("Index");
            }
            return View(contact);
        }
    }
}
