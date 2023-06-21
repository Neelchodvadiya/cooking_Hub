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
    public class RecipeLikesController : Controller
    {
        private readonly CookingHubContext _context;

        public RecipeLikesController(CookingHubContext context)
        {
            _context = context;
        }

		public async Task<IActionResult> CheckLikeStatus(string UserId, string recipeId)
		{
			try
			{
				// Find the BlogLike entry in the database that matches the provided UserId and BlogId
				var recipeLike = await _context.RecipeLikes.FirstOrDefaultAsync(bl => bl.UserId == UserId && bl.RecipeId == recipeId);

				if (recipeLike != null)
				{
					// User has liked the blog
					return Json(new { isLiked = true });
				}
				else
				{
					// User has not liked the blog
					return Json(new { isLiked = false });
				}
			}
			catch (Exception)
			{

				throw new Exception("Something is wrong.");
			}

		}

		// POST: AdminBlogLikes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

		public async Task<IActionResult> ToggleLike(string likeid, string userid, string recipeId, bool isLiked)
		{

			if (!isLiked)
			{
				var recipeLike = new RecipeLike();

				recipeLike.RecipeLikeId = likeid;
				recipeLike.UserId = userid;
				recipeLike.RecipeId = recipeId;
				recipeLike.LikedAt = DateTime.Now;

				_context.Add(recipeLike);
				await _context.SaveChangesAsync();
				return Json(new { isLiked = true });
			}
			else
			{
				var rLikes = await _context.RecipeLikes.FirstOrDefaultAsync(like => like.UserId == userid);

				if (rLikes != null)
				{
					_context.RecipeLikes.Remove(rLikes);
					await _context.SaveChangesAsync();
					return Json(new { isLiked = false });
				}
				return Json(new { isLiked = false });
			}



		}
		// GET: RecipeLikes
		public async Task<IActionResult> Index()
        {
            var cookingHubContext = _context.RecipeLikes.Include(r => r.Recipe).Include(r => r.User);
            return View(await cookingHubContext.ToListAsync());
        }

        // GET: RecipeLikes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.RecipeLikes == null)
            {
                return NotFound();
            }

            var recipeLike = await _context.RecipeLikes
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RecipeLikeId == id);
            if (recipeLike == null)
            {
                return NotFound();
            }

            return View(recipeLike);
        }

        // GET: RecipeLikes/Create
        public IActionResult Create()
        {
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: RecipeLikes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeLikeId,UserId,RecipeId,LikedAt")] RecipeLike recipeLike)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipeLike);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", recipeLike.RecipeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipeLike.UserId);
            return View(recipeLike);
        }

        // GET: RecipeLikes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.RecipeLikes == null)
            {
                return NotFound();
            }

            var recipeLike = await _context.RecipeLikes.FindAsync(id);
            if (recipeLike == null)
            {
                return NotFound();
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", recipeLike.RecipeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipeLike.UserId);
            return View(recipeLike);
        }

        // POST: RecipeLikes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RecipeLikeId,UserId,RecipeId,LikedAt")] RecipeLike recipeLike)
        {
            if (id != recipeLike.RecipeLikeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipeLike);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeLikeExists(recipeLike.RecipeLikeId))
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
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", recipeLike.RecipeId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", recipeLike.UserId);
            return View(recipeLike);
        }

        // GET: RecipeLikes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.RecipeLikes == null)
            {
                return NotFound();
            }

            var recipeLike = await _context.RecipeLikes
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RecipeLikeId == id);
            if (recipeLike == null)
            {
                return NotFound();
            }

            return View(recipeLike);
        }

        // POST: RecipeLikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.RecipeLikes == null)
            {
                return Problem("Entity set 'CookingHubContext.RecipeLikes'  is null.");
            }
            var recipeLike = await _context.RecipeLikes.FindAsync(id);
            if (recipeLike != null)
            {
                _context.RecipeLikes.Remove(recipeLike);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeLikeExists(string id)
        {
          return (_context.RecipeLikes?.Any(e => e.RecipeLikeId == id)).GetValueOrDefault();
        }
    }
}
