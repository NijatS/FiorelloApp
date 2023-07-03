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
                Include(x=>x.ProductTags).
                   ThenInclude(x=>x.Tag).
                Include(x => x.ProductCategories).
				   ThenInclude(x => x.Category).
				Include(x=>x.ProductImages).
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
            int i = 0;
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
            if (Product.FormFiles.Count == 0)
            {
                ModelState.AddModelError("", "Image must be added!");
                return View(Product);
            }
            foreach(var item in Product.FormFiles) {

                if (!Helper.isImage(item))
                {
                    ModelState.AddModelError("", "The format of file is image!!");
                    return View();
                }
                if (!Helper.isSizeOk(item, 1))
                {
                    ModelState.AddModelError("", "The size of image must less than 1mb!");
                    return View();
                }
                ProductImage productImage = new ProductImage
                {
                    CreatedDate = DateTime.Now,
                    Product = Product,
                    Image = item.CreateImage(_environment.WebRootPath, "assets/images"),
                    isMain = i==0?true:false,
                };
                await _context.ProductImages.AddAsync(productImage);
                i++;
            }

            foreach(var item in Product.CategoryIds)
            {
                if(!await _context.Categories.AnyAsync(x=>x.Id== item))
                {
                    ModelState.AddModelError("", "Invalid Category Id");
                    return View(Product);
                }
                ProductCategory productCategory = new ProductCategory
                {
                    CreatedDate = DateTime.Now,
                    Product = Product,
                    CategoryId = item
                };
                await _context.ProductCategories.AddAsync(productCategory);
            }
			foreach (var item in Product.TagIds)
			{
				if (!await _context.Tags.AnyAsync(x => x.Id == item))
				{
					ModelState.AddModelError("", "Invalid Tag Id");
					return View(Product);
				}
				ProductTag productTag = new ProductTag
				{
					CreatedDate = DateTime.Now,
					Product = Product,
					TagId = item
				};
				await _context.ProductTags.AddAsync(productTag);
			}

			await _context.AddAsync(Product);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

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
        //public async Task<IActionResult> Update(int id, Product Product)
        //{
       
        //    Product? UpdateProduct = await _context.Products.
        //        Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();

        //    if (Product == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return View(UpdateProduct);
        //    }

        //    if (Product.FormFile != null)
        //    {
        //        if (!Helper.isImage(Product.FormFile))
        //        {
        //            ModelState.AddModelError("FormFile", "Wronggg!");
        //            return View();
        //        }
        //        if (!Helper.isSizeOk(Product.FormFile, 1))
        //        {
        //            ModelState.AddModelError("FormFile", "Wronggg!");
        //            return View();
        //        }
        //        Helper.RemoveImage(_environment.WebRootPath, "assets/images", UpdateProduct.Image);
        //        UpdateProduct.Image = Product.FormFile.CreateImage(_environment.WebRootPath, "assets/images");
        //    }

        //    UpdateProduct.FullName = Product.FullName;
        //    UpdateProduct.Description = Product.Description;
        //    UpdateProduct.PositionId = Product.PositionId;
        //    UpdateProduct.UpdatedDate = DateTime.Now;
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
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
