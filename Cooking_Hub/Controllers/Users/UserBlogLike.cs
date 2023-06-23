using Cooking_Hub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cooking_Hub.Controllers.Users
{
    public class UserBlogLike : Controller
    {
        private readonly CookingHubContext _context;

        public UserBlogLike(CookingHubContext context)
        {
            _context = context;
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
               /* var likeCount = await _context.BlogLikes
                .CountAsync(l => l.BlogId == blogid);*/

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



        // GET: UserBlogLike
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserBlogLike/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserBlogLike/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserBlogLike/Create
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

        // GET: UserBlogLike/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserBlogLike/Edit/5
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

        // GET: UserBlogLike/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserBlogLike/Delete/5
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
