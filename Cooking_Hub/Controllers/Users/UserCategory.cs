using Cooking_Hub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cooking_Hub.Controllers.Users
{
    public class UserCategory : Controller
    {
        // GET: UserCategory
        private readonly CookingHubContext _context;
        public UserCategory(CookingHubContext context)
        {
            _context = context;
            

        }
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {

            var categories = _context.Categories
                        .GroupJoin(
                            _context.Recipes,
                            category => category.CategoryId,
                            recipe => recipe.CategoryId,
                            (category, recipes) => new RecipeViewModel
                            {
                                RecipeId = category.CategoryId,
                                Categoryname = category.CategoryName,
                                RecipeImage = category.CategoryImage,
                                TotalLikes = recipes.Count()
                            })
                        .OrderByDescending(result => result.TotalLikes);


            if (!string.IsNullOrEmpty(searchString))
            {
                categories = (IOrderedQueryable<RecipeViewModel>)categories.Where(category =>
                    category.Categoryname.ToLower().Contains(searchString.ToLower()));
            }

            if (searchString != null)
            {
                pageNumber = 1;
            }
            int pageSize = 9;
            categories = categories.OrderByDescending(result => result.TotalLikes);

            var paginatedList = await PaginatedList<RecipeViewModel>.CreateAsync(categories.AsQueryable(), pageNumber ?? 1, pageSize);
            ViewBag.SearchString = searchString; // Add this line to store the search query in the ViewBag
            if (paginatedList.Count == 0)
            {
                return View(paginatedList); // Return a "Not Found" error if no categories match the search criteria
            }

            return View(paginatedList);

        }

        // GET: UserCategory/Details/5
        public ActionResult Details(string id )
        {
            return View();
        }

        // GET: UserCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserCategory/Create
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

        // GET: UserCategory/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserCategory/Edit/5
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

        // GET: UserCategory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserCategory/Delete/5
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
