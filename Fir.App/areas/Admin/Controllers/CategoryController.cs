using Fir.App.Context;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllersb
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly FirDbContext _context;
        public CategoryController(FirDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories.
                Where(c =>!c.IsDeleted).ToListAsync();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) {
                return View();
            }
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("index","category");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Category? category= await _context.Categories.Where(x=>!x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
			if (category == null)
			{
                return NotFound();
			}
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Category postCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category? category = await _context.Categories.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            category.Name = postCategory.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "category");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _context.Categories.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }

}
