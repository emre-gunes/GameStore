using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using GameStore.Models;

namespace GameStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _context.Games.Include(g => g.Category).Take(4).ToListAsync();
            return View(games);
        }

        public async Task<IActionResult> Shop(int? categoryId, string searchKeyword)
        {
            var games = _context.Games.Include(g => g.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                games = games.Where(g => g.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                games = games.Where(g => g.Title.Contains(searchKeyword) || g.Description.Contains(searchKeyword));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(await games.ToListAsync());
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var game = await _context.Games.Include(g => g.Category).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
                return NotFound();

            return View(game);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
