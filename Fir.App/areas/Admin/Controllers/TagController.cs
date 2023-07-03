using Fir.App.Context;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllersb
{
    [Area("Admin")]
    public class TagController : Controller
    {

        private readonly FirDbContext _context;
        public TagController(FirDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Tag> Tags = await _context.Tags.
                Where(c =>!c.IsDeleted).ToListAsync();
            return View(Tags);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag Tag)
        {
            if (!ModelState.IsValid) {
                return View();
            }
            await _context.AddAsync(Tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("index","Tag");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Tag? Tag= await _context.Tags.Where(x=>!x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
			if (Tag == null)
			{
                return NotFound();
			}
            return View(Tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Tag postTag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag? Tag = await _context.Tags.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (Tag == null)
            {
                return NotFound();
            }
            Tag.Name = postTag.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "Tag");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Tag? Tag = await _context.Tags.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (Tag == null)
            {
                return NotFound();
            }

            Tag.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }

}
