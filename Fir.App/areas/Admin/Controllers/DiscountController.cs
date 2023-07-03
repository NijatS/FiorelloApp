using Fir.App.Context;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllersb
{
    [Area("Admin")]
    public class DiscountController : Controller
    {

        private readonly FirDbContext _context;
        public DiscountController(FirDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Discount> Discounts = await _context.Discounts.
                Where(c =>!c.IsDeleted).ToListAsync();
            return View(Discounts);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount Discount)
        {
            if (!ModelState.IsValid) {
                return View();
            }
            await _context.AddAsync(Discount);
            await _context.SaveChangesAsync();
            return RedirectToAction("index","Discount");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Discount? Discount= await _context.Discounts.Where(x=>!x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
			if (Discount == null)
			{
                return NotFound();
			}
            return View(Discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Discount postDiscount)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Discount? Discount = await _context.Discounts.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (Discount == null)
            {
                return NotFound();
            }
            Discount.Percent = postDiscount.Percent;
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "Discount");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Discount? Discount = await _context.Discounts.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (Discount == null)
            {
                return NotFound();
            }

            Discount.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }

}
