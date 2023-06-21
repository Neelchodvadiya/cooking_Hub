using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooking_Hub.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Drawing.Printing;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Cooking_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class adminBlogsController : Controller
    {
        private readonly CookingHubContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public adminBlogsController(CookingHubContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: adminBlogs
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {

            var blogs = _context.Blogs.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                bool? searchActive = null;

                if (searchString.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    searchActive = true;
                }
                else if (searchString.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                {
                    searchActive = false;
                }


                blogs = blogs.Where(b =>
                            (EF.Functions.Like(b.BlogshortDescription, $"%{searchString}%") ||
                            EF.Functions.Like(b.BlogContents, $"%{searchString}%") ||
                            EF.Functions.Like(b.BlogTitle.ToLower(), $"%{searchString.ToLower()}%")) ||
                            (searchActive.HasValue && b.BlogIsActive == searchActive.Value)
                        );

            }

            var blogComments = _context.BlogComments.GroupBy(bc => bc.BlogId)
                .Select(g => new { BlogId = g.Key, TotalComments = g.Count() })
                .ToList();

            var blogLikes = _context.BlogLikes.GroupBy(bl => bl.BlogId)
                .Select(g => new { BlogId = g.Key, TotalLikes = g.Count() })
                .ToList();

            int pageSize = 9;
            var viewModel = await PaginatedList<BlogViewModel>.CreateAsync(blogs
                .OrderByDescending(u => u.UpdatedAt)
                .Select(blog => new BlogViewModel
                {
                    BlogId = blog.BlogId,
                    BlogTitle = blog.BlogTitle,
                    BlogShortDescription = blog.BlogshortDescription,
                    BlogImage = blog.BlogImage,
                    BlogIsActive = blog.BlogIsActive,
                    CreatedAt = blog.CreatedAt,
                    TotalComments = _context.BlogComments.Count(b => b.BlogId == blog.BlogId),
                    TotalLikes = _context.BlogLikes.Count(l=> l.BlogId == blog.BlogId),
                }), pageNumber ?? 1, pageSize);

            ViewBag.SearchString = searchString;

            if (viewModel.Count == 0)
            {
                return View(viewModel); // Return a "Not Found" error if no categories match the search criteria
            }

            return View(viewModel);

        }


        // GET: adminBlogs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
        .Include(b => b.Category)
        .Include(b => b.User)
        .Include(b => b.BlogComments)
            .ThenInclude(c => c.User)
        .Include(b => b.BlogLikes)
        .FirstOrDefaultAsync(m => m.BlogId == id);


            if (blog == null)
            {
                return NotFound();
            }
            int commentCount = blog.BlogComments.Count();
            int likeCount = blog.BlogLikes.Count();

            ViewBag.CommentCount = commentCount;
            ViewBag.LikeCount = likeCount;

            return View(blog);
        }

        // GET: adminBlogs/Create
        public IActionResult Create()
        {
            string BlogId = Guid.NewGuid().ToString();
            var blog = new Blog { BlogId = BlogId };
           
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View(blog);
        }

        // POST: adminBlogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, IFormFile photo)
        {
            if (photo == null)
            {
                ModelState.AddModelError("ImageFilePath", "Please Select Image");
            }
            if (photo != null)
            {
                string filename = Path.GetFileName(photo.FileName);
                string uniqueFilename = $"{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now.Ticks}{Path.GetExtension(filename)}";
                string filepath = Path.Combine(hostEnvironment.WebRootPath, "UserImage", uniqueFilename);

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                blog.BlogImage = uniqueFilename;

            }


            _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
          
        }

        // GET: adminBlogs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", blog.CategoryId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", blog.UserId);
            return View(blog);
        }

        // POST: adminBlogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Blog blog, IFormFile photo)
        {
            if (id != blog.BlogId)
            {
                return NotFound();
            }
            if (blog.BlogIsActive == null)
            {
                blog.BlogIsActive = false;
            }
            if (photo == null)
            {
                var existingImage = await _context.Blogs.FindAsync(blog.BlogId);
                blog.BlogImage = existingImage.BlogImage;
               

                _context.Entry(existingImage).State = EntityState.Detached;
            }

            try
                {
                if (photo != null)
                {
                    string filename = Path.GetFileName(photo.FileName);
                    string uniqueFilename = $"{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now.Ticks}{Path.GetExtension(filename)}";
                    string filepath = Path.Combine(hostEnvironment.WebRootPath, "UserImage", uniqueFilename);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    blog.BlogImage = uniqueFilename;

                }
                var existingUser = await _context.Blogs.FindAsync(blog.BlogId);
                blog.CreatedAt = existingUser.CreatedAt;
                _context.Entry(existingUser).State = EntityState.Detached;

                _context.Update(blog);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              
           
        
            return View(blog);
        }

        // GET: adminBlogs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {

            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
        .Include(b => b.Category)
        .Include(b => b.User)
        .Include(b => b.BlogComments)
            .ThenInclude(c => c.User)
        .Include(b => b.BlogLikes)
        .FirstOrDefaultAsync(m => m.BlogId == id);


            if (blog == null)
            {
                return NotFound();
            }
            int commentCount = blog.BlogComments.Count();
            int likeCount = blog.BlogLikes.Count();

            ViewBag.CommentCount = commentCount;
            ViewBag.LikeCount = likeCount;

            return View(blog);
        }

        // POST: adminBlogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'CookingHubContext.Blogs'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                // Remove all comments associated with the blog
                var blogComments = _context.BlogComments.Where(bc => bc.BlogId == id);
                _context.BlogComments.RemoveRange(blogComments);

                // Remove all likes associated with the blog
                var blogLikes = _context.BlogLikes.Where(bl => bl.BlogId == id);
                _context.BlogLikes.RemoveRange(blogLikes);
                _context.Blogs.Remove(blog);
            }
          

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(string id)
        {
          return (_context.Blogs?.Any(e => e.BlogId == id)).GetValueOrDefault();
        }
    }
}
