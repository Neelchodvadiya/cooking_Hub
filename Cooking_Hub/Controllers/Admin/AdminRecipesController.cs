using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooking_Hub.Models;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Cooking_Hub.Controllers.Admin
{
	[Authorize(Roles = "Admin")]
	public class AdminRecipesController : Controller
	{
		private readonly CookingHubContext _context;
		private readonly IWebHostEnvironment hostEnvironment;

		public AdminRecipesController(CookingHubContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			this.hostEnvironment = hostEnvironment;
		}

		// GET: AdminRecipes
		public async Task<IActionResult> Index(string searchString, int? pageNumber)
		{
			var recipes = _context.Recipes.AsQueryable();

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

				recipes = recipes.Where(r =>
					(EF.Functions.Like(r.RecipeshortDescription, $"%{searchString}%") ||
					EF.Functions.Like(r.RecipeTitle.ToLower(), $"%{searchString.ToLower()}%")) ||
					(searchActive.HasValue && r.IsActive == searchActive.Value)
				);
			}

			var totalComments = _context.RecipeReviews.GroupBy(rr => rr.RecipeId)
				.Select(g => new { RecipeId = g.Key, TotalComments = g.Count() })
				.ToList();

			var totalLikes = _context.RecipeLikes.GroupBy(rl => rl.RecipeId)
				.Select(g => new { RecipeId = g.Key, TotalLikes = g.Count() })
				.ToList();

			int pageSize = 9;
			var viewModel = await PaginatedList<RecipeViewModel>.CreateAsync(recipes
				.OrderByDescending(u => u.CreatedAt)
				.Select(recipe => new RecipeViewModel
				{
					RecipeId = recipe.RecipeId,
					UserName = recipe.User.UserName,
					RecipeTitle = recipe.RecipeTitle,
					RecipeShortDescription = recipe.RecipeshortDescription,
					RecipeImage = recipe.RecipeImage,
					Views = recipe.Views,
					IsActive = recipe.IsActive,
					CreatedAt = recipe.CreatedAt,
					TotalComments = _context.RecipeReviews.Count(r => r.RecipeId == recipe.RecipeId),
					TotalLikes = _context.RecipeLikes.Count(rl => rl.RecipeId == recipe.RecipeId),
				}), pageNumber ?? 1, pageSize); ;

			ViewBag.SearchString = searchString;

			if (viewModel.Count == 0)
			{
				return View(viewModel); // Return a "Not Found" error if no recipes match the search criteria
			}

			return View(viewModel);
		}


		// GET: AdminRecipes/Details/5
		public async Task<IActionResult> Details(string id)
		{
			if (id == null || _context.Recipes == null)
			{
				return NotFound();
			}
			var recipe = await _context.Recipes
						   .Include(b => b.Category)
						   .Include(b => b.Cuisine)
						   .Include(b => b.User)
						   .Include(b => b.RecipeReviews)
							   .ThenInclude(c => c.User)
						   .Include(b => b.RecipeLikes)
						   .FirstOrDefaultAsync(m => m.RecipeId == id);

			int commentCount = recipe.RecipeReviews.Count();
			int likeCount = recipe.RecipeLikes.Count();

			ViewBag.CommentCount = commentCount;
			ViewBag.LikeCount = likeCount;

			ViewData["IngredientData"] = recipe.Ingridients;
			ViewData["NutritionData"] = recipe.Nutrition;
			if (recipe == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", recipe.CategoryId);
			ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineName", recipe.CuisineId);
			ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipe.UserId);
			

			recipe.Views++;

			// Attach the recipe entity and mark only the Views property as modified
			_context.Recipes.Attach(recipe);
			_context.Entry(recipe).Property(r => r.Views).IsModified = true;

			// Save the updated views count
			await _context.SaveChangesAsync();


			return View(recipe);
		}

		// GET: AdminRecipes/Create
		public IActionResult Create()
		{
			string Recipeid = Guid.NewGuid().ToString();
			var recipe = new Recipe { RecipeId = Recipeid };
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
			ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineName");
			ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
			return View(recipe);
		}

		// POST: AdminRecipes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Recipe recipe, string ingredientJson, string nutritionJson, IFormFile photo)
		{
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

					recipe.RecipeImage = uniqueFilename;

				}
				recipe.Views = 0;
				recipe.Ingridients = ingredientJson;
				recipe.Nutrition = nutritionJson;
				_context.Add(recipe);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{

				throw;
			}



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
			ViewData["IngredientData"] = recipe.Ingridients;
			ViewData["NutritionData"] = recipe.Nutrition;

			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", recipe.CategoryId);
			ViewData["CuisineId"] = new SelectList(_context.Cuisines, "CuisineId", "CuisineName", recipe.CuisineId);
			ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipe.UserId);
			return View(recipe);
		}

		// POST: AdminRecipes/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, Recipe recipe, string ingredientJson, string nutritionJson, IFormFile photo)
		{
			if (id != recipe.RecipeId)
			{
				return NotFound();
			}

			if (photo == null)
			{
				var existingImage = await _context.Recipes.FindAsync(recipe.RecipeId);
				recipe.RecipeImage = existingImage.RecipeImage;


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

					recipe.RecipeImage = uniqueFilename;

				}
				var existinrecipe = await _context.Recipes.FindAsync(recipe.RecipeId);
				recipe.CreatedAt = existinrecipe.CreatedAt;
				recipe.UserId = existinrecipe.UserId;
				_context.Entry(existinrecipe).State = EntityState.Detached;
				recipe.Ingridients = ingredientJson;
				recipe.Nutrition = nutritionJson;
				_context.Update(recipe);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
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


		}

		// GET: AdminRecipes/Delete/5
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null || _context.Recipes == null)
			{
				return NotFound();
			}

			
			var recipe = await _context.Recipes
						   .Include(b => b.Category)
						   .Include(b => b.Cuisine)
						   .Include(b => b.User)
						   .Include(b => b.RecipeReviews)
							   .ThenInclude(c => c.User)
						   .Include(b => b.RecipeLikes)
						   .FirstOrDefaultAsync(m => m.RecipeId == id);

			int commentCount = recipe.RecipeReviews.Count();
			int likeCount = recipe.RecipeLikes.Count();

			ViewBag.CommentCount = commentCount;
			ViewBag.LikeCount = likeCount;

			ViewData["IngredientData"] = recipe.Ingridients;
			ViewData["NutritionData"] = recipe.Nutrition;
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
				// Remove all comments associated with the blog
				var recComments = _context.RecipeReviews.Where(bc => bc.RecipeId == id);
				_context.RecipeReviews.RemoveRange(recComments);

				// Remove all likes associated with the blog
				var recLikes = _context.RecipeLikes.Where(bl => bl.RecipeId == id);
				_context.RecipeLikes.RemoveRange(recLikes);
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
