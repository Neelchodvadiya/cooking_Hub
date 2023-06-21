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

namespace Cooking_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogCommentsController : Controller
    {
        private readonly CookingHubContext _context;

        public AdminBlogCommentsController(CookingHubContext context)
        {
            _context = context;
        }

        // GET: AdminBlogComments
        public async Task<IActionResult> Index()
        {
            var cookingHubContext = _context.BlogComments.Include(b => b.Blog).Include(b => b.User);
            return View(await cookingHubContext.ToListAsync());
        }

        // GET: AdminBlogComments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.BlogComments == null)
            {
                return NotFound();
            }

            var blogComment = await _context.BlogComments
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (blogComment == null)
            {
                return NotFound();
            }

            return View(blogComment);
        }

        // GET: AdminBlogComments/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: AdminBlogComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogComment blogComment)
        {

            _context.Add(blogComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "adminBlogs", new { id = blogComment.BlogId });

        }
		public async Task<IActionResult> CreateRecipecomment(RecipeReview recipeReview)
		{

			_context.Add(recipeReview);
			await _context.SaveChangesAsync();
			return RedirectToAction("Details", "AdminRecipes", new { id = recipeReview.RecipeId });

		}
		public async Task<IActionResult> RecipecommentDelete(string id)
		{
			if (_context.RecipeReviews == null)
			{
				return Problem("Entity set 'CookingHubContext.RecipeReviews'  is null.");
			}
			var recipeReviewss = await _context.RecipeReviews.FindAsync(id);
			if (recipeReviewss != null)
			{
				_context.RecipeReviews.Remove(recipeReviewss);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Details", "AdminRecipes", new { id = recipeReviewss.RecipeId });
		}


		// GET: AdminBlogComments/Edit/5
		public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.BlogComments == null)
            {
                return NotFound();
            }

            var blogComment = await _context.BlogComments.FindAsync(id);
            if (blogComment == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", blogComment.BlogId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", blogComment.UserId);
            return View(blogComment);
        }

        // POST: AdminBlogComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CommentId,ParentId,UserId,BlogId,BcommentContents,CreatedAt")] BlogComment blogComment)
        {
            if (id != blogComment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogCommentExists(blogComment.CommentId))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", blogComment.BlogId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", blogComment.UserId);
            return View(blogComment);
        }

        // GET: AdminBlogComments/Delete/5
      /*  public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.BlogComments == null)
            {
                return NotFound();
            }

            var blogComment = await _context.BlogComments
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (blogComment == null)
            {
                return NotFound();
            }

            return View(blogComment);
        }*/

        // POST: AdminBlogComments/Delete/5
    
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.BlogComments == null)
            {
                return Problem("Entity set 'CookingHubContext.BlogComments'  is null.");
            }
            var blogComment = await _context.BlogComments.FindAsync(id);
            if (blogComment != null)
            {
                _context.BlogComments.Remove(blogComment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "adminBlogs", new { id = blogComment.BlogId });
        }

        private bool BlogCommentExists(string id)
        {
          return (_context.BlogComments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
