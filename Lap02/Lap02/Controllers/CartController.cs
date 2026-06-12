using Lap02.Extensions;
using Lap02.Models;
using Lap02.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lap02.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "SHOPPING_CART";

        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(
            IProductRepository productRepository,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        private List<ShoppingCartItem> GetCart()
        {
            return HttpContext.Session.GetObjectFromJson<List<ShoppingCartItem>>(CartSessionKey)
                   ?? new List<ShoppingCartItem>();
        }

        private void SaveCart(List<ShoppingCartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }

        public IActionResult Index()
        {
            var cart = GetCart();

            ViewBag.TotalAmount = cart.Sum(x => x.Total);

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }

            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item == null)
            {
                cart.Add(new ShoppingCartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? "Sản phẩm",
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Increase(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrease(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                CustomerName = user?.FullName ?? "",
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber ?? "",
                Address = user?.Address ?? "",
                PaymentMethod = "COD"
            };

            ViewBag.TotalAmount = cart.Sum(x => x.Total);

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = GetCart();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TotalAmount = cart.Sum(x => x.Total);
                return View(order);
            }

            var userId = _userManager.GetUserId(User);

            order.UserId = userId;
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";
            order.TotalAmount = cart.Sum(x => x.Total);

            if (string.IsNullOrEmpty(order.PaymentMethod))
            {
                order.PaymentMethod = "COD";
            }

            order.OrderDetails = cart.Select(item => new OrderDetail
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ImageUrl = item.ImageUrl,
                UnitPrice = item.Price,
                Quantity = item.Quantity,
                TotalPrice = item.Total
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("Success", new { id = order.Id });
        }

        [Authorize]
        public async Task<IActionResult> Success(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (!User.IsInRole(SD.Role_Admin) && order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }
    }
}
