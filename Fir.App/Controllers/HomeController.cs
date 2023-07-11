
using Fir.App.Context;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Fir.App.Controllers
{
    public class HomeController : Controller
    {

        private readonly FirDbContext _context;
        private readonly UserManager<AppUser> _user;

        public HomeController(FirDbContext context, UserManager<AppUser> user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var JsonBasket = Request.Cookies["basket"];
                AppUser appUser = await  _user.FindByNameAsync(User.Identity.Name);
                if (JsonBasket != null)
                {
                    Basket basket = new Basket
                    {
                        CreatedDate = DateTime.Now,
                        AppUser = appUser,
                    };
                    await _context.AddAsync(basket);
                    List<BasketItemVM>? basketItemVMs = JsonConvert.DeserializeObject<List<BasketItemVM>>(JsonBasket);

                    foreach (var item in basketItemVMs)
                    {
                        BasketItem basketItem = new BasketItem
                        {
                            Basket = basket,
                            Count = item.Count,
                            CreatedDate = DateTime.Now,
                            ProductId = item.ProductId,
                        };
                        await _context.AddAsync(basketItem);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            HomeVM homeVM=new HomeVM();
           homeVM.categories = await _context.Categories.Where(c => !c.IsDeleted)
                .ToListAsync();
            homeVM.blogs = await _context.Blogs.Where(c => !c.IsDeleted)
               .ToListAsync();
            homeVM.employees = await _context.Employees.Where(c => !c.IsDeleted)
               .Include(c=>c.Position)
                .ToListAsync();
            homeVM.products = await _context.Products.Where(c => !c.IsDeleted).
                Include(c => c.ProductCategories)
                 .ThenInclude(c => c.Category).
                Include(c => c.ProductImages).
                Include(c => c.Discount).
                ToListAsync();
            return View(homeVM);
        }


    }
}