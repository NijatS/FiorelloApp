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
            foreach (var item in Product.FormFiles)
            {

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
                    isMain = i == 0 ? true : false,
                };
                await _context.ProductImages.AddAsync(productImage);
                i++;
            }
            Product.DiscountId = Product.DiscountId == 0 ? null : Product.DiscountId;

            foreach (var item in Product.CategoryIds)
            {
                if (!await _context.Categories.AnyAsync(x => x.Id == item))
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
			ViewBag.Categories = await _context.Categories.
			 Where(p => !p.IsDeleted).ToListAsync();
			ViewBag.Tags = await _context.Tags
				.Where(p => !p.IsDeleted).ToListAsync();
			ViewBag.Discounts = await _context.Discounts.
				Where(p => !p.IsDeleted).ToListAsync();

            Product? Product = await _context.Products.
				  Where(c => !c.IsDeleted && c.Id == id).
				Include(x => x.ProductTags.Where(x=>!x.IsDeleted)).
                   ThenInclude(x => x.Tag).
                Include(x => x.ProductCategories.Where(x => !x.IsDeleted)).
                   ThenInclude(x => x.Category).
                Include(x => x.ProductImages.Where(x => !x.IsDeleted))
              .FirstOrDefaultAsync();
			if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(int id,Product product)
		{
			ViewBag.Categories = await _context.Categories.
			 Where(p => !p.IsDeleted).ToListAsync();
			ViewBag.Tags = await _context.Tags
				.Where(p => !p.IsDeleted).ToListAsync();
			ViewBag.Discounts = await _context.Discounts.
				Where(p => !p.IsDeleted).ToListAsync();

			Product? UpdatedProduct = await _context.Products.
                AsNoTracking().
				  Where(c => !c.IsDeleted && c.Id == id).
				Include(x => x.ProductTags.Where(x => !x.IsDeleted)).
				   ThenInclude(x => x.Tag).
				Include(x => x.ProductCategories.Where(x => !x.IsDeleted)).
				   ThenInclude(x => x.Category).
				Include(x => x.ProductImages.Where(x => !x.IsDeleted))
			  .FirstOrDefaultAsync();
			if (UpdatedProduct is null)
			{
				return NotFound();
			}
            if (!ModelState.IsValid)
            {
                return View(UpdatedProduct);
            }
            List<ProductCategory> RemoveableCategory = await _context.ProductCategories.
                Where(x => !product.CategoryIds.Contains(x.CategoryId)).ToListAsync();

            _context.ProductCategories.RemoveRange(RemoveableCategory);
            foreach(var item in product.CategoryIds)
            {
                if (_context.ProductCategories.Where(x => x.ProductId == id && x.CategoryId == item).Count()>0)
                    continue;

                await _context.ProductCategories.AddAsync(new ProductCategory
                {
                    ProductId = id,
                    CategoryId = item
                });
            }
			List<ProductTag> RemoveableTag = await _context.ProductTags.
			Where(x => !product.TagIds.Contains(x.TagId)).ToListAsync();

			_context.ProductTags.RemoveRange(RemoveableTag);
			foreach (var item in product.TagIds)
			{
				if (_context.ProductTags.Where(x => x.ProductId == id && x.TagId == item).Count() > 0)
					continue;

				await _context.ProductTags.AddAsync(new ProductTag
				{
					ProductId = id,
					TagId = item
				});
			}

            product.DiscountId = product.DiscountId == 0 ? null : product.DiscountId;
            if(product.FormFiles is not null && product.FormFiles.Count > 0)
            {
                foreach (var item in product.FormFiles)
                {
                    if (!Helper.isImage(item))
                    {
                        ModelState.AddModelError("", "The format of file is image!!");
                        return View(UpdatedProduct);
                    }
                    if (!Helper.isSizeOk(item, 1))
                    {
                        ModelState.AddModelError("", "The size of image must less than 1mb!");
                        return View(UpdatedProduct);
                    }
                    ProductImage productImage = new ProductImage
                    {
                        CreatedDate = DateTime.Now,
                        Product = product,
                        Image = item.CreateImage(_environment.WebRootPath, "assets/images"),
                    };
                    await _context.ProductImages.AddAsync(productImage);
                }
			}
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> SetAsMainImage(int id)
        {
            ProductImage? productImage = await _context.ProductImages.FindAsync(id);

            if(productImage is null)
            {
                return Json(new
                {
                    status = 404
                }) ;
			}
            productImage.isMain = true;

            ProductImage? productImage1 = await _context.ProductImages.Where(x=>x.isMain && x.ProductId==productImage.ProductId).FirstOrDefaultAsync();
            productImage1.isMain = false;

            await _context.SaveChangesAsync();
            return Json(new { 
            status=200
            });
        }
        public async Task<IActionResult> RemoveImage(int id)
        {
            ProductImage? productImage = await _context.ProductImages.
                Where(x => x.Id == id && !x.IsDeleted).
                FirstOrDefaultAsync();
            if(productImage is null)
            {
				return Json(new
				{
					status = 404,
					desc = "Image not found"
				});
			}
            if (productImage.isMain)
            {
                return Json(new
                {
                    status = 400,
                    desc = "Main image is not deleted"
                });
            }
            productImage.IsDeleted = true;
            await _context.SaveChangesAsync();
			return Json(new
			{
				status = 200
			});
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
