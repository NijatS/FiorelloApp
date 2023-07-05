
using Fir.App.Context;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fir.App.Controllers
{
    public class HomeController : Controller
    {

        private readonly FirDbContext _context;

		public HomeController(FirDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
        {
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