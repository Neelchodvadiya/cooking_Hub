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
    public class AdminUsersController : Controller
    {
        private readonly CookingHubContext _context;

        public AdminUsersController(CookingHubContext context)
        {
            _context = context;
        }

		// GET: AdminUsersss
		public async Task<IActionResult> Index(string searchString, int? pageNumber, string filter)
		{

			ViewBag.ShowFilter = true;
			var users = _context.AspNetUsers.AsQueryable();

			if (!String.IsNullOrEmpty(searchString))
			{
				users = users.Where(s =>
					s.UserName.ToLower().Contains(searchString.ToLower()) ||
					s.Email.ToLower().Contains(searchString.ToLower()) ||
					s.PhoneNumber.ToLower().Contains(searchString.ToLower())
				);
			}

			if (!String.IsNullOrEmpty(filter))
			{
				if (filter == "active")
				{
					users = users.Where(u => u.IsActive == true);
				}
				else if (filter == "inactive")
				{
					users = users.Where(u => u.IsActive == false);
				}
			}

			int pageSize = 6;
			var viewModel = await PaginatedList<UserViewModel>.CreateAsync(users.Select(user => new UserViewModel
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				IsActive = user.IsActive,
				TotalCreatedRecipes = _context.Recipes.Count(r => r.UserId == user.Id),
				TotalLikedRecipes = _context.RecipeLikes.Count(rl => rl.UserId == user.Id),
				TotalReviewedRecipes = _context.RecipeReviews.Count(rr => rr.UserId == user.Id)
			}), pageNumber ?? 1, pageSize);
			ViewBag.SearchString = searchString;
			return View(viewModel);
		}

		// GET: AdminUsers/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // GET: AdminUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,Gender,ImageFilePath,IsActive,CreatedAt")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aspNetUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aspNetUser);
        }

        // GET: AdminUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return NotFound();
            }
            return View(aspNetUser);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,Gender,ImageFilePath,IsActive,CreatedAt")] AspNetUser aspNetUser)
        {
            if (id != aspNetUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspNetUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserExists(aspNetUser.Id))
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
            return View(aspNetUser);
        }

        // GET: AdminUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AspNetUsers == null)
            {
                return Problem("Entity set 'CookingHubContext.AspNetUsers'  is null.");
            }
            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser != null)
            {
                _context.AspNetUsers.Remove(aspNetUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetUserExists(string id)
        {
          return (_context.AspNetUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
