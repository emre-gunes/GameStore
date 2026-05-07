using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using GameStore.Models;

namespace GameStore.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LibraryController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var library = await _context.UserLibraries
                .Include(l => l.Game)
                .Where(l => l.AppUserId == user.Id && !l.IsRefunded)
                .ToListAsync();

            return View(library);
        }

        [HttpPost]
        public async Task<IActionResult> PlayGame(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var libraryItem = await _context.UserLibraries
                .FirstOrDefaultAsync(l => l.Id == id && l.AppUserId == user.Id && !l.IsRefunded);

            if (libraryItem != null)
            {
                libraryItem.PlayTimeInHours += 1;
                _context.Update(libraryItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Refund(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var libraryItem = await _context.UserLibraries
                .Include(l => l.Game)
                .FirstOrDefaultAsync(l => l.Id == id && l.AppUserId == user.Id && !l.IsRefunded);

            if (libraryItem != null)
            {
                if (libraryItem.PlayTimeInHours < 2)
                {
                    libraryItem.IsRefunded = true;
                    _context.Update(libraryItem);

                    if (libraryItem.Game != null)
                    {
                        libraryItem.Game.StockQuantity += 1;
                        _context.Games.Update(libraryItem.Game);
                    }

                    // Note: In a real system, you would refund the OrderItem as well.
                    
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
