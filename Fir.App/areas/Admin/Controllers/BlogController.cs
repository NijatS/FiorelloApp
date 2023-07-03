using Fir.App.Context;
using Fir.App.Extentions;
using Fir.App.Helpers;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly FirDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BlogController(FirDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await _context.Blogs.
               Where(c => !c.IsDeleted).ToListAsync();
            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (blog.FormFile == null)
            {
                ModelState.AddModelError("FormFile", "Wrong!");
                return View();
            }




            if (!Helper.isImage(blog.FormFile))
            {
                ModelState.AddModelError("FormFile","Wronggg!");
                return View();
            }
            if (!Helper.isSizeOk(blog.FormFile,1))
            {
                ModelState.AddModelError("FormFile", "Wronggg!");
                return View();
            }
          

            blog.Image = blog.FormFile.CreateImage(_environment.WebRootPath,"assets/images/");
            blog.CreatedDate=DateTime.Now;
            await _context.AddAsync(blog);

            await _context.SaveChangesAsync();
            return RedirectToAction("index", "blog");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Blog? blog = await _context.Blogs.
                  Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Blog blog)
        {

             Blog? Updateblog = await _context.Blogs.
                  Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();

            if (blog == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(Updateblog);
            }

            if(blog.FormFile != null)
            {
                if (!Helper.isImage(blog.FormFile))
                {
                    ModelState.AddModelError("FormFile", "Wronggg!");
                    return View();
                }
                if (!Helper.isSizeOk(blog.FormFile, 1))
                {
                    ModelState.AddModelError("FormFile", "Wronggg!");
                    return View();
                }
                Helper.RemoveImage(_environment.WebRootPath, "assets/images", Updateblog.Image);
                Updateblog.Image=blog.FormFile.CreateImage(_environment.WebRootPath,"assets/images");
            }

            Updateblog.Title = blog.Title;
           Updateblog.Description=blog.Description;
            Updateblog.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Blog? blog = await _context.Blogs.
                  Where(c => !c.IsDeleted && c.Id == id).FirstOrDefaultAsync();
            if (blog == null)
            {
                return NotFound();
            }
            blog.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
