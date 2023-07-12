using Fir.App.Context;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly FirDbContext _context;
        public OrderController(UserManager<AppUser> userManager, FirDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> CheckOut()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Basket ? basket = await _context.Baskets.
                             Include(x => x.BasketItems.Where(y => !y.IsDeleted)).
                             ThenInclude(x => x.Product).
                             ThenInclude(x => x.ProductImages).
                             Include(x => x.BasketItems.Where(y => !y.IsDeleted)).
                             ThenInclude(x => x.Product).
                             ThenInclude(x => x.Discount).
                             Where(x => !x.IsDeleted && x.AppUserId == user.Id).FirstOrDefaultAsync();

            return View(basket);
        }
    }
}
