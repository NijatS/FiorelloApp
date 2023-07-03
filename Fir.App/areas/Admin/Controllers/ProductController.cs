using Fir.App.Context;
using Fir.App.Extentions;
using Fir.App.Helpers;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly FirDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(FirDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> Products = await _context.Products.
                Where(c => !c.IsDeleted).ToListAsync();
            return View(Products);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.
                Where(p=>!p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags
                .Where(p=>!p.IsDeleted).ToListAsync();
            ViewBag.Discounts = await _context.Discounts.
                Where(p=>!p.IsDeleted).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product Product)
        {
            ViewBag.Categories = await _context.Categories.
                Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags
                .Where(p => !p.IsDeleted).ToListAsync();
            ViewBag.Discounts = await _context.Discounts.
                Where(p => !p.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (Product.FormFiles.Count == null)
            {
                ModelState.AddModelError("FormFile", "Wrong!");
                return View();
            }
            foreach(var item in Product.FormFiles) {

                if (!Helper.isImage(item))
                {
                    ModelState.AddModelError("FormFile", "Wronggg!");
                    return View();
                }
                if (!Helper.isSizeOk(Product.FormFile, 1))
                {
                    ModelState.AddModelError("FormFile", "Wronggg!");
                    return View();
                }
            }
            Product.Image = Product.FormFile.CreateImage(_environment.WebRootPath, "assets/images");

            await _context.AddAsync(Product);

            await _context.SaveChangesAsync();
            return RedirectToAction("index", "Product");

        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Positions = await _context.Positions.Where(p => !p.IsDeleted ).ToListAsync();
            Product? Product = await _context.Products.
                  Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Product Product)
        {
       
            Product? UpdateProduct = await _context.Products.
                Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();

            if (Product == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(UpdateProduct);
            }

            if (Product.FormFile != null)
            {
                if (!Helper.isImage(Product.FormFile))
                {
                    ModelState.AddModelError("FormFile", "Wronggg!");
                    return View();
                }
                if (!Helper.isSizeOk(Product.FormFile, 1))
                {
                    ModelState.AddModelError("FormFile", "Wronggg!");
                    return View();
                }
                Helper.RemoveImage(_environment.WebRootPath, "assets/images", UpdateProduct.Image);
                UpdateProduct.Image = Product.FormFile.CreateImage(_environment.WebRootPath, "assets/images");
            }

            UpdateProduct.FullName = Product.FullName;
            UpdateProduct.Description = Product.Description;
            UpdateProduct.PositionId = Product.PositionId;
            UpdateProduct.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Product? Product = await _context.Products.
               Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();
       
            if (Product == null)
            {
                return NotFound();
            }

            Product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
