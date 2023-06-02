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
using System.Security.Cryptography.Pkcs;

namespace Cooking_Hub.Controllers.Admin
{
	[Authorize(Roles = "Admin")]
	public class AdminCategoriesController : Controller
    {
        private readonly CookingHubContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public AdminCategoriesController(CookingHubContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: AdminCategories
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {

            var categories = _context.Categories.AsQueryable();

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

                categories = categories.Where(c =>
                    c.CategoryName.ToLower().Contains(searchString.ToLower()) ||
                    (searchActive == null || c.CategoryIsActive == searchActive));
            }

            if (searchString != null)
            {
                pageNumber = 1;
            }
            int pageSize = 8;
            var paginatedList = await PaginatedList<Category>.CreateAsync(categories.AsNoTracking(), pageNumber ?? 1, pageSize);

            ViewBag.SearchString = searchString; // Add this line to store the search query in the ViewBag

            return View(paginatedList);
            return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'CookingHubContext.Categories'  is null.");
        }

        // GET: AdminCategories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: AdminCategories/Create
        public IActionResult Create()
        {
            string categoryId = Guid.NewGuid().ToString();
            var category = new Category { CategoryId = categoryId };
            return View(category);
        }

        // POST: AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryIsActive,CategoryImage")] Category category, IFormFile photo)
        {
           
            if (ModelState.IsValid)
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

					category.CategoryImage = uniqueFilename;

				}
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: AdminCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AdminCategories/Edit/{id}")]
        public async Task<IActionResult> Edit(string id,  Category category, IFormFile photo)
        {

            if (id != category.CategoryId)
            {
                return NotFound();
            }
            
            if (photo == null)
            {
                var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
                category.CategoryImage = existingCategory.CategoryImage;
             
                _context.Entry(existingCategory).State = EntityState.Detached;
            }

            /*if (!ModelState.IsValid)
            {*/
               

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

                        category.CategoryImage = uniqueFilename;

                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                    return View(category);
                  
                }
                
            }
            

            /* }*/

        }

        // GET: AdminCategories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'CookingHubContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(string id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
