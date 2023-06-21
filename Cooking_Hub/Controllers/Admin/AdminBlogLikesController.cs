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
	public class AdminBlogLikesController : Controller
    {
        private readonly CookingHubContext _context;

        public AdminBlogLikesController(CookingHubContext context)
        {
            _context = context;
        }

        // GET: AdminBlogLikes
        public async Task<IActionResult> Index()
        {
            var cookingHubContext = _context.BlogLikes.Include(b => b.Blog).Include(b => b.User);
            return View(await cookingHubContext.ToListAsync());
        }

        // GET: AdminBlogLikes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.BlogLikes == null)
            {
                return NotFound();
            }

            var blogLike = await _context.BlogLikes
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.LikeId == id);
            if (blogLike == null)
            {
                return NotFound();
            }

            return View(blogLike);
        }

        // GET: AdminBlogLikes/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        public async Task<IActionResult> CheckLikeStatus(string UserId, string BlogId)
        {
            try
            {
                // Find the BlogLike entry in the database that matches the provided UserId and BlogId
                var blogLike = await _context.BlogLikes.FirstOrDefaultAsync(bl => bl.UserId == UserId && bl.BlogId == BlogId);

                if (blogLike != null)
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

        public async Task<IActionResult> ToggleLike(string likeid, string userid, string blogid, bool isLiked)
        {

            if (!isLiked)
            {
                var blogLike = new BlogLike();

                blogLike.LikeId = likeid;
                blogLike.UserId = userid;
                blogLike.BlogId = blogid;
                blogLike.LikedAt = DateTime.Now;

                _context.Add(blogLike);
                await _context.SaveChangesAsync();
                return Json(new { isLiked = true });
            }
            else
            {
                var blogLikes = await _context.BlogLikes.FirstOrDefaultAsync(like => like.UserId == userid);

                if (blogLikes != null)
                {
                    _context.BlogLikes.Remove(blogLikes);
                    await _context.SaveChangesAsync();
                    return Json(new { isLiked = false });
                }
                return Json(new { isLiked = false });
            }



        }

        // POST: AdminBlogLikes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LikeId,UserId,BlogId,LikedAt")] BlogLike blogLike,bool isLiked)
        {
            if (ModelState.IsValid)
            {
                if (isLiked)
                {
                    _context.Add(blogLike);
                    await _context.SaveChangesAsync();
                    return Json(new { isLiked = true });
                }
                else
                {
                    var blogLikes = await _context.BlogLikes.FirstOrDefaultAsync(like => like.UserId == blogLike.UserId);

                    if (blogLikes != null)
                    {
                        _context.BlogLikes.Remove(blogLikes);
                        await _context.SaveChangesAsync();
                        return Json(new { isLiked = false });
                    }
                }
               
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", blogLike.BlogId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", blogLike.UserId);
            return View(blogLike);
        }

        // GET: AdminBlogLikes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.BlogLikes == null)
            {
                return NotFound();
            }

            var blogLike = await _context.BlogLikes.FindAsync(id);
            if (blogLike == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", blogLike.BlogId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", blogLike.UserId);
            return View(blogLike);
        }

        // POST: AdminBlogLikes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LikeId,UserId,BlogId,LikedAt")] BlogLike blogLike)
        {
            if (id != blogLike.LikeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogLike);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogLikeExists(blogLike.LikeId))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", blogLike.BlogId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", blogLike.UserId);
            return View(blogLike);
        }

        // GET: AdminBlogLikes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.BlogLikes == null)
            {
                return NotFound();
            }

            var blogLike = await _context.BlogLikes
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.LikeId == id);
            if (blogLike == null)
            {
                return NotFound();
            }

            return View(blogLike);
        }

        // POST: AdminBlogLikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.BlogLikes == null)
            {
                return Problem("Entity set 'CookingHubContext.BlogLikes'  is null.");
            }
            var blogLike = await _context.BlogLikes.FindAsync(id);
            if (blogLike != null)
            {
                _context.BlogLikes.Remove(blogLike);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogLikeExists(string id)
        {
          return (_context.BlogLikes?.Any(e => e.LikeId == id)).GetValueOrDefault();
        }
    }
}
