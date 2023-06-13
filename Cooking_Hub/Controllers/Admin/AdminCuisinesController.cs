using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooking_Hub.Models;

namespace Cooking_Hub.Controllers.Admin
{
    public class AdminCuisinesController : Controller
    {
        private readonly CookingHubContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public AdminCuisinesController(CookingHubContext context , IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: AdminCuisines
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
		{

			var cuisines = _context.Cuisines.AsQueryable();

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

				cuisines = cuisines.Where(c =>
						 (c.CuisineName.ToLower().Contains(searchString.ToLower()) ||
						 (searchActive.HasValue && c.IsActive == searchActive.Value)));
			}

			if (searchString != null)
			{
				pageNumber = 1;
			}
			int pageSize = 8;
			var paginatedList = await PaginatedList<Cuisine>.CreateAsync(cuisines.AsNoTracking(), pageNumber ?? 1, pageSize);

			ViewBag.SearchString = searchString; // Add this line to store the search query in the ViewBag
			if (paginatedList.Count == 0)
			{
				return View(paginatedList); // Return a "Not Found" error if no categories match the search criteria
			}

			return View(paginatedList);

		}

		// GET: AdminCuisines/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Cuisines == null)
            {
                return NotFound();
            }

            var cuisine = await _context.Cuisines
                .FirstOrDefaultAsync(m => m.CuisineId == id);
            if (cuisine == null)
            {
                return NotFound();
            }

            return View(cuisine);
        }

        // GET: AdminCuisines/Create
        public IActionResult Create()
        {
			string cuisineId = Guid.NewGuid().ToString();
			var cusine = new Cuisine { CuisineId = cuisineId };
			return View(cusine);
        }

        // POST: AdminCuisines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("CuisineId,CuisineName,CuisineImage,IsActive")] Cuisine cuisine, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                // Check if category name already exists
                if (_context.Cuisines.Any(c => c.CuisineName == cuisine.CuisineName))
                {
                    ModelState.AddModelError("CuisineName", "CuisineName Name already exists at this Cuisine level");
                    return View(cuisine);
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

                    cuisine.CuisineImage = uniqueFilename;

                }
                _context.Add(cuisine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cuisine);
        }

        // GET: AdminCuisines/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Cuisines == null)
            {
                return NotFound();
            }

            var cuisine = await _context.Cuisines.FindAsync(id);
            if (cuisine == null)
            {
                return NotFound();
            }
            return View(cuisine);
        }

        // POST: AdminCuisines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AdminCuisines/Edit/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("CuisineId,CuisineName,CuisineImage,IsActive")] Cuisine cuisine,IFormFile photo)
        {
            if (id != cuisine.CuisineId)
            {
                return NotFound();
            }
            
            if (photo == null)
            {
                var existingcuisine = await _context.Cuisines.FindAsync(cuisine.CuisineId);
                cuisine.CuisineImage = existingcuisine.CuisineImage;
                _context.Entry(existingcuisine).State = EntityState.Detached;
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

                        cuisine.CuisineImage = uniqueFilename;

                }
                _context.Update(cuisine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuisineExists(cuisine.CuisineId))
                {
                    return NotFound();
                }
                else
                {
                    return View(cuisine);

                }

            }
        }

        // GET: AdminCuisines/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Cuisines == null)
            {
                return NotFound();
            }

            var cuisine = await _context.Cuisines
                .FirstOrDefaultAsync(m => m.CuisineId == id);
            if (cuisine == null)
            {
                return NotFound();
            }

            return View(cuisine);
        }

        // POST: AdminCuisines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Cuisines == null)
            {
                return Problem("Entity set 'CookingHubContext.Cuisines'  is null.");
            }
            var cuisine = await _context.Cuisines.FindAsync(id);
            if (cuisine != null)
            {
                _context.Cuisines.Remove(cuisine);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuisineExists(string id)
        {
          return (_context.Cuisines?.Any(e => e.CuisineId == id)).GetValueOrDefault();
        }
    }
}
