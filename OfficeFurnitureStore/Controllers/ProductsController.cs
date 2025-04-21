using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeFurnitureStore.Models;

namespace OfficeFurnitureStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly OfficeFurnitureStoreContext _context;

        public ProductsController(OfficeFurnitureStoreContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var officeFurnitureStoreContext = _context.Products.Include(p => p.Category);
            return View(await officeFurnitureStoreContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Comments = await _context.Comments
                .Include(c => c.Customer)
                .Where(c => c.ProductId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(product);
        }

        // POST: Products/Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int ProductId, string Content)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để bình luận.";
                return RedirectToAction("Details", new { id = ProductId });
            }

            var hasOrdered = await _context.OrderDetails
                .AnyAsync(od => od.Order.CustomerId == customerId && od.ProductId == ProductId);
            if (!hasOrdered)
            {
                TempData["Error"] = "Bạn chỉ có thể bình luận sản phẩm đã đặt hàng.";
                return RedirectToAction("Details", new { id = ProductId });
            }

            var comment = new Comment
            {
                ProductId = ProductId,
                CustomerId = customerId.Value,
                Content = Content,
                CreatedAt = DateTime.Now
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Bình luận đã được gửi.";
            return RedirectToAction("Details", new { id = ProductId });
        }
    }
}