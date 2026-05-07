using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GameStore.Data;
using GameStore.Extensions;
using GameStore.Models;

namespace GameStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult Add(int id)
        {
            var game = _context.Games.Find(id);
            if (game != null)
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
                
                // Avoid adding the same game twice
                if (!cart.Any(c => c.GameId == id))
                {
                    cart.Add(new CartItem
                    {
                        GameId = game.Id,
                        Title = game.Title,
                        Price = game.Price,
                        ImageUrl = game.ImageUrl
                    });
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                cart.RemoveAll(c => c.GameId == id);
                HttpContext.Session.Set("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart == null || !cart.Any()) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                AppUserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Price),
                Status = "Completed"
            };

            foreach (var item in cart)
            {
                var game = await _context.Games.FindAsync(item.GameId);
                if (game != null && game.StockQuantity > 0)
                {
                    game.StockQuantity -= 1;
                    _context.Games.Update(game);

                    order.OrderItems.Add(new OrderItem
                    {
                        GameId = item.GameId,
                        Price = item.Price
                    });

                    _context.UserLibraries.Add(new UserLibrary
                    {
                        AppUserId = user.Id,
                        GameId = item.GameId,
                        PurchaseDate = DateTime.Now,
                        PlayTimeInHours = 0,
                        IsRefunded = false
                    });
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index", "Library");
        }
    }
}
