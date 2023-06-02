using Cooking_Hub.Data;
using Cooking_Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace Cooking_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {

        private readonly UserManager<CookingHubUser> _userManager;
		private readonly CookingHubContext _dbContext;
		public DashboardController(UserManager<CookingHubUser> userManager, CookingHubContext dbContext)
        {
            _userManager = userManager;
			_dbContext = dbContext;
		}

        // GET: DashboardController
        // GET: DashboardController
        public async Task<ActionResult> Index()
        {
            try
            {
				var user = await _userManager.GetUserAsync(User);
				string currentUserName = user.UserName;
                //user chart
                ViewBag.currentUserName = currentUserName;
				var currentMonth = DateTime.Now.Month;
				var currentYear = DateTime.Now.Year;

				var users = await _userManager.Users
					.Where(u => u.CreatedAt.Month == currentMonth && u.CreatedAt.Year == currentYear)
					.ToListAsync();

				var calendar = CultureInfo.InvariantCulture.Calendar;

				var userGroups = new List<dynamic>();
				var currentMonthStartDate = new DateTime(currentYear, currentMonth, 1);
				var currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

				var totalWeeksInMonth = calendar.GetWeekOfYear(currentMonthEndDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) -
										calendar.GetWeekOfYear(currentMonthStartDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

				for (int week = 1; week <= totalWeeksInMonth; week++)
				{
					var weekStartDate = currentMonthStartDate.AddDays((week - 1) * 7);
					var weekEndDate = weekStartDate.AddDays(6);
					var userCount = users.Count(u => u.CreatedAt >= weekStartDate && u.CreatedAt <= weekEndDate);
					userGroups.Add(new { Week = week, UserCount = userCount });
				}

				var weekLabels = userGroups.Select(group => "Week " + group.Week).ToArray();
				var userCounts = userGroups.Select(group => group.UserCount).ToArray();

				ViewBag.WeekLabels = weekLabels;
				ViewBag.UserCounts = userCounts;

				//User Count

				int totalUserCount = await _userManager.Users.CountAsync();
				int activeUserCount = await _userManager.Users.CountAsync(u => u.IsActive == true);
                int inactiveUserCount = await _userManager.Users.CountAsync(u => u.IsActive == false);


				double activeUserPercentage = (double)totalUserCount / activeUserCount * 100;
				double inactiveUserPercentage = (double)totalUserCount / inactiveUserCount * 100;


                ViewBag.totalUserCount = totalUserCount;
                ViewBag.activeUserPercentage = activeUserPercentage;
                ViewBag.inactiveUserPercentage = inactiveUserPercentage;
				ViewBag.ActiveUserCount = activeUserCount;
				ViewBag.InactiveUserCount = inactiveUserCount;

				// Recipes Count
				int activeRecipeCount = await _dbContext.Recipes.CountAsync(r => r.IsActive == true);
				int inactiveRecipeCount = await _dbContext.Recipes.CountAsync(r => r.IsActive == false);

				ViewBag.ActiveRecipeCount = activeRecipeCount;
				ViewBag.InactiveRecipeCount = inactiveRecipeCount;

                //Blogs Count
				int activeBlogCount = await _dbContext.Blogs.CountAsync(b => b.BlogIsActive == true);
				int inactiveBlogCount = await _dbContext.Blogs.CountAsync(b => b.BlogIsActive == false);

				ViewBag.ActiveBlogCount = activeBlogCount;
				ViewBag.InactiveBlogCount = inactiveBlogCount;

				//Categories Count
				int activeCategoryCount = await _dbContext.Categories.CountAsync(c => c.CategoryIsActive == true);
				int inactiveCategoryCount = await _dbContext.Categories.CountAsync(c => c.CategoryIsActive == false);

				ViewBag.ActiveCategoryCount = activeCategoryCount;
				ViewBag.InactiveCategoryCount = inactiveCategoryCount;


                return View();
            }
            catch (Exception ex)
            {
              
                Console.WriteLine(ex); // Log the exception
                return View("Error"); // Show an error view
            }
        }


        // GET: DashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashboardController/Create
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
		public async Task<IActionResult> Pie()
		{
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var users = await _userManager.Users
                .Where(u => u.CreatedAt.Month == currentMonth && u.CreatedAt.Year == currentYear)
                .ToListAsync();

            var calendar = CultureInfo.InvariantCulture.Calendar;

            var userGroups = new List<dynamic>();
            var currentMonthStartDate = new DateTime(currentYear, currentMonth, 1);
            var currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

            var totalWeeksInMonth = calendar.GetWeekOfYear(currentMonthEndDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) -
                                    calendar.GetWeekOfYear(currentMonthStartDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

            for (int week = 1; week <= totalWeeksInMonth; week++)
            {
                var weekStartDate = currentMonthStartDate.AddDays((week - 1) * 7);
                var weekEndDate = weekStartDate.AddDays(6);
                var userCount = users.Count(u => u.CreatedAt >= weekStartDate && u.CreatedAt <= weekEndDate);
                userGroups.Add(new { Week = week, UserCount = userCount });
            }

            var weekLabels = userGroups.Select(group => "Week " + group.Week).ToArray();
            var userCounts = userGroups.Select(group => group.UserCount).ToArray();

            ViewBag.WeekLabels = weekLabels;
            ViewBag.UserCounts = userCounts;

            return View();

           


        }

        // GET: DashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashboardController/Edit/5
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

        // GET: DashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashboardController/Delete/5
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
