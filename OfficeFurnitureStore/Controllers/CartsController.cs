using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeFurnitureStore.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace OfficeFurnitureStore.Controllers
{
    public class CartsController : Controller
    {
        private readonly OfficeFurnitureStoreContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public CartsController(OfficeFurnitureStoreContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: Carts/Index
        public async Task<IActionResult> Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Index", "LoginC");
            }

            var carts = _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.CustomerId == customerId);
            return View(await carts.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: Carts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,CustomerId,ProductId,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", cart.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Add/5
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để sử dụng chức năng này.";
                return RedirectToAction("Index", "LoginC");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null || product.Status != "Available")
            {
                TempData["Error"] = "Sản phẩm không tồn tại hoặc đã hết hàng.";
                return RedirectToAction("Index", "Products");
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.ProductId == id && c.CustomerId == customerId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
                _context.Carts.Update(existingCartItem);
            }
            else
            {
                var cart = new Cart
                {
                    CustomerId = customerId.Value,
                    ProductId = id,
                    Quantity = 1
                };
                await _context.Carts.AddAsync(cart);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Index", "Products");
        }

        // POST: Carts/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để cập nhật giỏ hàng." });
            }

            var cart = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.CustomerId == customerId);
            if (cart == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            if (quantity < 1)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0.", currentQuantity = cart.Quantity });
            }

            if (cart.Product.StockQuantity < quantity)
            {
                return Json(new { success = false, message = $"Sản phẩm {cart.Product.ProductName} chỉ còn {cart.Product.StockQuantity} trong kho.", currentQuantity = cart.Quantity });
            }

            cart.Quantity = quantity;
            _context.Update(cart);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật số lượng thành công!" });
        }

        // POST: Carts/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xóa sản phẩm.";
                return RedirectToAction("Index", "LoginC");
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CartId == id && c.CustomerId == customerId);
            if (cart == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại trong giỏ hàng.";
                return RedirectToAction(nameof(Index));
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Carts/ConfirmAddress
        [HttpGet]
        public async Task<IActionResult> ConfirmAddress()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đặt hàng.";
                return RedirectToAction("Index", "LoginC");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index", "Products");
            }

            return View(customer);
        }

        // POST: Carts/PlaceOrderWithAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrderWithAddress(string newAddress, string paymentMethod)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đặt hàng.";
                return RedirectToAction("Index", "LoginC");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index", "Products");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, không thể thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            // Validate stock for all cart items
            foreach (var item in cartItems)
            {
                if (item.Product == null)
                {
                    TempData["Error"] = $"Sản phẩm trong giỏ hàng không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }
                if (item.Product.StockQuantity < item.Quantity)
                {
                    TempData["Error"] = $"Sản phẩm {item.Product.ProductName} chỉ còn {item.Product.StockQuantity} trong kho.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update customer address if provided
                if (!string.IsNullOrEmpty(newAddress))
                {
                    customer.Address = newAddress;
                    _context.Customers.Update(customer);
                    await _context.SaveChangesAsync();
                }

                // Create new order
                var order = new Order
                {
                    CustomerId = customerId.Value,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = "Đang xử lý"
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order details and update product stock
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    };
                    _context.OrderDetails.Add(orderDetail);

                    // Update product stock
                    item.Product.StockQuantity -= item.Quantity;
                    item.Product.OrderedQuantity += item.Quantity;
                    _context.Products.Update(item.Product);
                }

                // Remove cart items
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                // Handle payment method
                if (paymentMethod == "MoMo")
                {
                    var paymentUrl = await CreateMoMoPayment(order.OrderId, totalAmount);
                    if (!string.IsNullOrEmpty(paymentUrl))
                    {
                        HttpContext.Session.SetInt32("PendingOrderId", order.OrderId);
                        await transaction.CommitAsync();
                        return Redirect(paymentUrl);
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        TempData["Error"] = "Không thể tạo yêu cầu thanh toán MoMo.";
                        return RedirectToAction(nameof(ConfirmAddress));
                    }
                }
                else
                {
                    order.Status = "Tiền mặt";
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Đơn hàng đã được đặt thành công.";
                    return RedirectToAction("Index", "Products");
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Lỗi khi đặt hàng: {ex.Message}";
                return RedirectToAction(nameof(ConfirmAddress));
            }
        }

        // GET: Carts/MoMoCallback
        [HttpGet]
        public async Task<IActionResult> MoMoCallback(string orderId, string resultCode, string message)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => "MM" + o.OrderId == orderId);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(ConfirmAddress));
            }

            if (resultCode == "0")
            {
                order.Status = "Chuyển khoản";
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thanh toán MoMo thành công. Đơn hàng đã được đặt.";
                return RedirectToAction("Index", "Products");
            }
            else
            {
                TempData["Error"] = $"Thanh toán MoMo thất bại: {message}";
                return RedirectToAction(nameof(ConfirmAddress));
            }
        }

        // Hàm tạo yêu cầu thanh toán MoMo
        private async Task<string> CreateMoMoPayment(int orderId, decimal amount)
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO_ATM_DEV";
            string accessKey = "w9gEg8bjA2AM2Cvr";
            string secretKey = "mD9QAVi4cm9N844jh5Y2tqjWaaJoGVFM";
            string orderInfo = $"Thanh toán đơn hàng #{orderId}";
            string redirectUrl = "https://localhost:7032/Carts/MoMoCallback";
            string ipnUrl = "https://localhost:7032/Carts/MoMoCallback";
            string requestId = Guid.NewGuid().ToString();
            string orderIdStr = $"MM{orderId}";
            string amountStr = ((int)amount).ToString();

            var rawData = $"accessKey={accessKey}&amount={amountStr}&extraData=&ipnUrl={ipnUrl}&orderId={orderIdStr}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=payWithATM";
            var signature = HmacSha256(secretKey, rawData);

            var requestData = new
            {
                partnerCode,
                partnerName = "OfficeFurnitureStore",
                storeId = "FurnitureStore",
                requestId,
                amount = amountStr,
                orderId = orderIdStr,
                orderInfo,
                redirectUrl,
                ipnUrl,
                lang = "vi",
                requestType = "payWithATM",
                autoCapture = true,
                extraData = "",
                signature
            };

            using var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return responseData.payUrl;
            }

            return null;
        }

        // Hàm tạo chữ ký HMAC SHA256
        private string HmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}