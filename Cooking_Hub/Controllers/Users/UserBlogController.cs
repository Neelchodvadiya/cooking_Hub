using Cooking_Hub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cooking_Hub.Controllers.Users
{
    public class UserBlogController : Controller
    {
        // GET: UserBlogController
        private readonly CookingHubContext _context;
      

        public UserBlogController(CookingHubContext context)
        {
            _context = context;
           
        }


        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {


            //LATEST BLOG
            var latestBlogs = _context.Blogs

                .OrderByDescending(b => b.CreatedAt)
                .Take(4)
                .ToList();

            ViewData["LatestBlogs"] = latestBlogs;

            //Top 3 Liked Blogs

            var topLikedBlogs = _context.BlogLikes
          .GroupBy(l => l.BlogId)
          .Select(g => new
          {
              BlogId = g.Key,
              LikeCount = g.Count()
          })
          .OrderByDescending(g => g.LikeCount)
          .Take(3)
          .Join(_context.Blogs.Where(b => b.BlogIsActive == true), // Add the filter for active blogs
              like => like.BlogId,
              blog => blog.BlogId,
              (like, blog) => new BlogViewModel
              {
                  BlogId = blog.BlogId,
                  BlogTitle = blog.BlogTitle,
                  BlogImage = blog.BlogImage,
                  BlogShortDescription =blog.BlogshortDescription,
                  Blogcategory = blog.Category.CategoryName,
              })
          .ToList();

            ViewData["TopLikedBlogs"] = topLikedBlogs;

            //Category With its count 
            var categoryRecipeCount = _context.Categories
                        .GroupJoin(
                            _context.Recipes,
                            category => category.CategoryId,
                            recipe => recipe.CategoryId,
                            (category, recipes) => new RecipeViewModel
                            {
                                RecipeId = category.CategoryId,
                                Categoryname = category.CategoryName,
                                TotalLikes = recipes.Count()
                            })
                        .OrderByDescending(result => result.TotalLikes)
                        .Take(7)
                        .ToList();
            ViewData["CategoryRecipeCount"] = categoryRecipeCount;

            //Blog List
            var categorrry = _context.Categories;
            var blogs = _context.Blogs.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {



                blogs = blogs
                         .Join(
                             categorrry,
                             blog => blog.CategoryId,
                             category => category.CategoryId,
                             (blog, category) => new { Blog = blog, Category = category }
                         )
                         .Where(b =>
                             EF.Functions.Like(b.Blog.BlogshortDescription, $"%{searchString}%") ||
                             EF.Functions.Like(b.Blog.BlogContents, $"%{searchString}%") ||
                             EF.Functions.Like(b.Blog.BlogTitle.ToLower(), $"%{searchString.ToLower()}%") ||
                             EF.Functions.Like(b.Category.CategoryName, $"%{searchString}%")
                         )
                         .Select(b => b.Blog);
            }

            var blogComments = _context.BlogComments.GroupBy(bc => bc.BlogId)
                .Select(g => new { BlogId = g.Key, TotalComments = g.Count() })
            .ToList();

            var blogLikes = _context.BlogLikes.GroupBy(bl => bl.BlogId)
                .Select(g => new { BlogId = g.Key, TotalLikes = g.Count() })
                .ToList();

            int pageSize = 4;
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
                    TotalLikes = _context.BlogLikes.Count(l => l.BlogId == blog.BlogId),
                }), pageNumber ?? 1, pageSize);

            ViewBag.SearchString = searchString;
            ViewBag.ShowFilter = true;
            if (viewModel.Count == 0)
            {
                return View(viewModel); // Return a "Not Found" error if no categories match the search criteria
            }

            return View(viewModel);

        }

        // GET: UserBlogController/Details/5
        public async Task<IActionResult> Details(string id)
        {

            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }
            //LATEST BLOG
            var latestBlogs = _context.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .Take(4)
                .ToList();

            ViewData["LatestBlogs"] = latestBlogs;

            //Top 3 Liked Blogs

            var topLikedBlogs = _context.BlogLikes
          .GroupBy(l => l.BlogId)
          .Select(g => new
          {
              BlogId = g.Key,
              LikeCount = g.Count()
          })
          .OrderByDescending(g => g.LikeCount)
          .Take(3)
          .Join(_context.Blogs.Where(b => b.BlogIsActive == true), // Add the filter for active blogs
              like => like.BlogId,
              blog => blog.BlogId,
              (like, blog) => new BlogViewModel
              {
                  BlogId = blog.BlogId,
                  BlogTitle = blog.BlogTitle,
                  BlogImage = blog.BlogImage,
                  BlogShortDescription = blog.BlogshortDescription,
                  Blogcategory = blog.Category.CategoryName,
              })
          .ToList();

            ViewData["TopLikedBlogs"] = topLikedBlogs;

            //Category With its count 
            var categoryRecipeCount = _context.Categories
                        .GroupJoin(
                            _context.Recipes,
                            category => category.CategoryId,
                            recipe => recipe.CategoryId,
                            (category, recipes) => new RecipeViewModel
                            {
                                RecipeId = category.CategoryId,
                                Categoryname = category.CategoryName,
                                TotalLikes = recipes.Count()
                            })
                        .OrderByDescending(result => result.TotalLikes)
                        .Take(7)
                        .ToList();
            ViewData["CategoryRecipeCount"] = categoryRecipeCount;

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

        // GET: UserBlogController/Create
        

           

       

        // POST: UserBlogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogComment blogComment)
    {
        try
            {
                _context.Add(blogComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "UserBlog", new { id = blogComment.BlogId });
            }
            catch
            {
                return View();
            }
        }

        // GET: UserBlogController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserBlogController/Edit/5
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

        // GET: UserBlogController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserBlogController/Delete/5
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
