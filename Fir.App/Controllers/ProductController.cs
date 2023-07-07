using Fir.App.Context;
using Fir.App.Services.Interfaces;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fir.App.Controllers
{
    public class ProductController : Controller
    {
        private readonly FirDbContext _context;
        private readonly IBasketService _service;
        private readonly IHttpContextAccessor _contextAccessor;
        public ProductController(FirDbContext context, IHttpContextAccessor contextAccessor, IBasketService service)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _service = service;
        }

        public async Task<IActionResult> Detail(int id)
        {
            Product? product = await _context.Products.Where(x => x.Id == id && !x.IsDeleted)
                .Include(x=>x.ProductCategories)
                   .ThenInclude(x=>x.Category)
                .Include(x => x.ProductTags)
                   .ThenInclude(x => x.Tag)
                   .Include(x => x.Discount)
                 .Include(x => x.ProductImages)
                .FirstOrDefaultAsync();
            if(product is null) { 
            return NotFound();
            }
            ProductVM productVM = new ProductVM
            {
                Product = product
            };

            return View(productVM);
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            await _service.AddBasket(id);
            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> RemoveBasket(int id)
        {
            await _service.Remove(id);
            return RedirectToAction("index", "home");
        }
    }
}
