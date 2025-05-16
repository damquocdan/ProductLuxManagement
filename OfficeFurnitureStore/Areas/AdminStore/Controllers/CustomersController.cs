using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeFurnitureStore.Models;

namespace OfficeFurnitureStore.Areas.AdminStore.Controllers
{
    [Area("AdminStore")]
    public class CustomersController : Controller
    {
        private readonly OfficeFurnitureStoreContext _context;

        public CustomersController(OfficeFurnitureStoreContext context)
        {
            _context = context;
        }

        // GET: AdminStore/Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: AdminStore/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: AdminStore/Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminStore/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Username,Password,FullName,Email,Phone,Address,Avatar,CreatedAt")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Any() && files[0].Length > 0)
                {
                    var file = files[0];
                    var fileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\avatars", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        customer.Avatar = "/images/avatars/" + fileName;
                    }
                }
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: AdminStore/Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Username,Password,FullName,Email,Phone,Address,Avatar,CreatedAt")] Customer customer, IFormFile AvatarFile)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            // Log ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ValidationErrors"] = string.Join("; ", errors);
                return View(customer);
            }

            try
            {
                // Retrieve the existing customer
                var existingCustomer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerId == id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                // Handle avatar file upload
                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("AvatarFile", "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif).");
                        return View(customer);
                    }

                    // Validate file size (e.g., max 5MB)
                    if (AvatarFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("AvatarFile", "Kích thước file không được vượt quá 5MB.");
                        return View(customer);
                    }

                    // Generate unique file name
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars", fileName);

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AvatarFile.CopyToAsync(stream);
                    }

                    // Update avatar path
                    customer.Avatar = "/images/avatars/" + fileName;

                    // Delete old avatar file
                    if (!string.IsNullOrEmpty(existingCustomer.Avatar))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCustomer.Avatar.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                else
                {
                    // Retain existing avatar
                    customer.Avatar = existingCustomer.Avatar;
                }

                // Update customer
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                {
                    return NotFound();
                }
                ModelState.AddModelError("", "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng thử lại.");
                return View(customer);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi lưu dữ liệu: {ex.Message}");
                return View(customer);
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: AdminStore/Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: AdminStore/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
