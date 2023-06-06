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
using Microsoft.AspNetCore.Identity;
using Cooking_Hub.Data;
using Microsoft.Extensions.Hosting;

namespace Cooking_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUsers : Controller
    {
        private readonly CookingHubContext _context;
        private readonly IUserStore<CookingHubUser> _userStore;
        private readonly UserManager<CookingHubUser> _userManager;
        private readonly IWebHostEnvironment hostEnvironment;

        public AdminUsers(CookingHubContext context, UserManager<CookingHubUser> userManager)
        {
            _context = context;
           
            _userManager = userManager;
        }

        // GET: AdminUsers
        public async Task<IActionResult> Index(string searchString, int? pageNumber, string filter)
        {

            ViewBag.ShowFilter = true;
            var users = _context.AspNetUsers.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s =>
                    s.UserName.ToLower().Contains(searchString.ToLower()) ||
                    s.Email.ToLower().Contains(searchString.ToLower()) ||
                    s.Gender.ToLower().Contains(searchString.ToLower()) ||
                    s.FirstName.ToLower().Contains(searchString.ToLower()) ||
                    s.LastName.ToLower().Contains(searchString.ToLower()) ||
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
            var viewModel = await PaginatedList<UserViewModel>.CreateAsync(users
                .OrderByDescending(u => u.CreatedAt) // Order by CreatedAt in descending order
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ImageFilePath = user.ImageFilePath,
                    TotalCreatedRecipes = _context.Recipes.Count(r => r.UserId == user.Id),
                    TotalLikedRecipes = _context.RecipeLikes.Count(rl => rl.UserId == user.Id),
                    TotalReviewedRecipes = _context.RecipeReviews.Count(rr => rr.UserId == user.Id)
                }), pageNumber ?? 1, pageSize);

            ViewBag.SearchString = searchString;

            if (viewModel.Count == 0)
            {
                return View(viewModel); // Return a "Not Found" error if no categories match the search criteria
            }
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
            string id = Guid.NewGuid().ToString();
            var aspNetUser = new AspNetUser { Id = id };
            return View(aspNetUser);
            
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,Gender,ImageFilePath,IsActive,CreatedAt,FirstName,LastName")] AspNetUser aspNetUser ,IFormFile photo)
        {
            if (string.IsNullOrEmpty(aspNetUser.UserName))
            {
                ModelState.AddModelError("UserName", "Please enter a username.");
            }

            if (string.IsNullOrEmpty(aspNetUser.Email))
            {
                ModelState.AddModelError("Email", "Please enter an email address.");
            }
            var existingUser = await _userManager.FindByNameAsync(aspNetUser.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "The provided UserName already exists.");
            }

            existingUser = await _userManager.FindByEmailAsync(aspNetUser.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "The provided Email already exists.");
            }

            if (ModelState.IsValid)
            {
               

                string userid = aspNetUser.Id;
              aspNetUser.SecurityStamp = Guid.NewGuid().ToString();
                aspNetUser.ConcurrencyStamp = Guid.NewGuid().ToString(); // Generate new ConcurrencyStamp

                if (photo != null)
                {
                    string filename = Path.GetFileName(photo.FileName);
                    string uniqueFilename = $"{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now.Ticks}{Path.GetExtension(filename)}";
                    string filepath = Path.Combine(hostEnvironment.WebRootPath, "UserImage", uniqueFilename);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    aspNetUser.ImageFilePath = uniqueFilename;

                }
                _context.Add(aspNetUser);
                await _context.SaveChangesAsync();
                try
                {
                    var user = await _userManager.FindByIdAsync(userid);

                    if (!await _userManager.IsInRoleAsync(user, "User"))
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    ModelState.AddModelError("", "An error occurred while adding the user to the role. Please try again.");
                    // Log the exception or handle it as needed

                    return View(aspNetUser);
                }
               
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,Gender,ImageFilePath,IsActive,CreatedAt,FirstName,LastName")] AspNetUser aspNetUser)
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
