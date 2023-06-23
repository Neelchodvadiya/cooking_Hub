using Cooking_Hub.Data;
using Cooking_Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace Cooking_Hub.Controllers.Users
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly CookingHubContext _context;
		private readonly IUserStore<CookingHubUser> _userStore;
		private readonly UserManager<CookingHubUser> _userManager;


		public HomeController(ILogger<HomeController> logger, CookingHubContext context)
        {
            _logger = logger;
			_context = context;
		}
		
		public IActionResult Index()
        {
           

            //topRatedRecipes based on a average review
            var topRatedRecipes = _context.Recipes
            .Where(r => r.IsActive == true)
            .Select(recipe => new
            {
                Recipe = recipe,
                AverageRating = _context.RecipeReviews
                    .Where(review => review.RecipeId == recipe.RecipeId)
                    .Average(review => review.Rating),
                TotalLikes = _context.RecipeLikes
                    .Count(like => like.RecipeId == recipe.RecipeId),
                TotalReviews = _context.RecipeReviews
                    .Count(review => review.RecipeId == recipe.RecipeId),
                UserName = _context.AspNetUsers
                    .Where(user => user.Id == recipe.UserId)
                    .Select(user => user.UserName)
                    .FirstOrDefault()
            })
            .OrderByDescending(r => r.AverageRating)
            .Take(3)
            .Select(r => new RecipeViewModel
            {
                RecipeId = r.Recipe.RecipeId,
               
                RecipeTitle = r.Recipe.RecipeTitle,

                RecipeShortDescription = r.Recipe.RecipeshortDescription,
              
                RecipeImage = r.Recipe.RecipeImage,
                IsActive = r.Recipe.IsActive,
               Categoryname = r.Recipe.Category.CategoryName,
                TotalLikes = r.TotalLikes,
                TotalComments = r.TotalReviews,
                UserName = r.UserName
            })
            .ToList();


            ViewData["TopRatedRecipes"] = topRatedRecipes;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }


        public async Task<IActionResult> CreateContactUs(Contact contact)
        {
            try
            {
                contact.ContactId = Guid.NewGuid().ToString();
                contact.CreatedAt = DateTime.Now;   
                _context.Add(contact);
                await _context.SaveChangesAsync();
                               
            }
            catch (Exception)
            {

                throw;
            }
           return RedirectToAction(nameof(Index));
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}