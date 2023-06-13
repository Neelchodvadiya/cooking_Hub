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
    public class AdminRecipesController : Controller
    {
        private readonly CookingHubContext _context;

        public AdminRecipesController(CookingHubContext context)
        {
            _context = context;
        }

        // GET: AdminRecipes
        public async Task<IActionResult> Index()
        {
            var cookingHubContext = _context.Recipes.Include(r => r.Category).Include(r => r.Cuisine).Include(r => r.User);
            return View(await cookingHubContext.ToListAsync());
        }

        // GET: AdminRecipes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Cuisine)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: AdminRecipes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineId");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: AdminRecipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,UserId,CategoryId,RecipeTitle,CuisineId,RecipeshortDescription,RecipeDescription,PreparationTime,CookingTime,Serving,Views,Ingridients,Nutrition,RecipeImage,IsActive,CreatedAt,UpdatedAt")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
              


                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", recipe.CategoryId);
            ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineId", recipe.CuisineId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // GET: AdminRecipes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", recipe.CategoryId);
            ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineId", recipe.CuisineId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // POST: AdminRecipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RecipeId,UserId,CategoryId,RecipeTitle,CuisineId,RecipeshortDescription,RecipeDescription,PreparationTime,CookingTime,Serving,Views,Ingridients,Nutrition,RecipeImage,IsActive,CreatedAt,UpdatedAt")] Recipe recipe)
        {
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RecipeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", recipe.CategoryId);
            ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineId", recipe.CuisineId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // GET: AdminRecipes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Cuisine)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: AdminRecipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Recipes == null)
            {
                return Problem("Entity set 'CookingHubContext.Recipes'  is null.");
            }
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(string id)
        {
          return (_context.Recipes?.Any(e => e.RecipeId == id)).GetValueOrDefault();
        }
    }
}
