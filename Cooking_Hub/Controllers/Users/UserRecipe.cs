using Cooking_Hub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cooking_Hub.Controllers.Users
{
    public class UserRecipe : Controller
    {
        private readonly CookingHubContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public UserRecipe(CookingHubContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }
        // GET: UserRecipe
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            //LATEST BLOG
            var latestRecipe = _context.Recipes
                 .Include(b => b.User)
                  .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Take(4)
                .ToList();

            ViewData["latestRecipe"] = latestRecipe;

            var topLikedRecipes = _context.RecipeLikes
                                .GroupBy(l => l.RecipeId)
                                .Select(g => new
                                {
                                    RecipeId = g.Key,
                                    LikeCount = g.Count()
                                })
                                .OrderByDescending(g => g.LikeCount)
                                .Take(3)
                                .Join(_context.Recipes.Where(r => r.IsActive == true), // Add the filter for active recipes
                                    like => like.RecipeId,
                                    recipe => recipe.RecipeId,
                                    (like, recipe) => new RecipeViewModel
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
                                        TotalLikes = like.LikeCount,
                                        Categoryname = recipe.Category.CategoryName
                                    })
                                .ToList();

            ViewData["TopLikedRecipes"] = topLikedRecipes;



            var recipes = _context.Recipes.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
               

                recipes = recipes.Where(r =>
                    (EF.Functions.Like(r.RecipeshortDescription, $"%{searchString}%") ||
                    EF.Functions.Like(r.RecipeTitle.ToLower(), $"%{searchString.ToLower()}%"))               
                );
            }

            var totalComments = _context.RecipeReviews.GroupBy(rr => rr.RecipeId)
                .Select(g => new { RecipeId = g.Key, TotalComments = g.Count() })
            .ToList();

            var totalLikes = _context.RecipeLikes.GroupBy(rl => rl.RecipeId)
                .Select(g => new { RecipeId = g.Key, TotalLikes = g.Count() })
                .ToList();

            int pageSize = 8;
            var viewModel = await PaginatedList<RecipeViewModel>.CreateAsync(recipes
                .OrderByDescending(u => u.CreatedAt).Where(r => r.IsActive == true)
                .Select(recipe => new RecipeViewModel
                {
                    RecipeId = recipe.RecipeId,
                    UserName = recipe.User.UserName,
                    RecipeTitle = recipe.RecipeTitle,
                    Categoryname = recipe.Category.CategoryName,
                    RecipeShortDescription = recipe.RecipeshortDescription,
                    RecipeImage = recipe.RecipeImage,
                    Views = recipe.Views,
                    IsActive = recipe.IsActive,
                    CreatedAt = recipe.CreatedAt,
                    TotalComments = _context.RecipeReviews.Count(r => r.RecipeId == recipe.RecipeId),
                    TotalLikes = _context.RecipeLikes.Count(rl => rl.RecipeId == recipe.RecipeId),
                }), pageNumber ?? 1, pageSize); 

            ViewBag.SearchString = searchString;

            if (viewModel.Count == 0)
            {
                return View(viewModel); // Return a "Not Found" error if no recipes match the search criteria
            }

            return View(viewModel);
        }

        // GET: UserRecipe/Details/5
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

        // GET: UserRecipe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRecipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRecipe/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserRecipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRecipe/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserRecipe/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
