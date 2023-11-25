using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using proj.Models;
using proj.Controllers;
using proj.Areas;
using System.Security.Claims;
using proj.Services;

namespace proj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        public Skill SkillInput { get; set; }
        public Input Input { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IDatabaseRepository _db;
        public string ReturnUrl { get; set; }

        public AdminDashboardController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IDatabaseRepository databaseRepository)
        {
            _userManager = userManager;
            _db = databaseRepository;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Resumes()
        {
            List<Resume> r = _db.GetAllResumesWithUser();
            return View(r);
        }
        [HttpGet]
        public IActionResult View(string id)
        {
            List<Resume> r = _db.GetResumeForUser(id);

            if (r != null)
            {
                return RedirectToAction("Resume", new { id = r[0].Id });
            }
            else
            {
                TempData["Error"] = "No resume to view.";
                // Handle the case where the resume with the given id is not found.
                return RedirectToAction("Users");
            }
        }
        [HttpGet]
        public IActionResult Resume(int id)
        {
            Resume r = _db.GetResumeWithSkillsById(id);
            return View(r);
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Delete_ResumeAsync(int id)
        {
            await _db.DeleteResume(id);
            TempData["Message"] = "Resume deleted successfully";
            return RedirectToAction("Resumes");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Users()
        {
            var usersWithRoles = _userManager.Users
                .Select(user => new UsersView
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result
                })
                .ToList();

            return View(usersWithRoles);

        }
        [HttpGet]
        public IActionResult Skills()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Input model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Check if the "Admin" role exists, and create it if not
                    var adminRoleExists = await _roleManager.RoleExistsAsync("Admin");
                    if (!adminRoleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    // Assign the "Admin" role to the user
                    await _userManager.AddToRoleAsync(user, "Admin");

                    // You can sign in the user if needed
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    Console.WriteLine("new admin created succesfully!");
                    return RedirectToAction("Index", "AdminDashboard");
                    // return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["Message"] = "Admin added successfully!";
                return RedirectToAction("Index", "AdminDashboard");
            }
            TempData["Error"] = "Registration failed";
            return RedirectToAction("Index", "AdminDashboard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string userId)
        {
            // Your logic to delete the user based on the id
            var userToDelete = _userManager.FindByIdAsync(userId).Result;

            if (userToDelete != null)
            {
                var result = _userManager.DeleteAsync(userToDelete).Result;

                if (result.Succeeded)
                {
                    // Deletion was successful
                    // Optionally, you can add a success message to TempData
                    TempData["Message"] = "User deleted successfully.";
                }
                else
                {
                    // Handle errors, add error messages to TempData or ModelState
                    TempData["Error"] = "Error deleting user.";
                }
            }
            else
            {
                // User not found
                TempData["Error"] = "User not found.";
            }

            return RedirectToAction("Users");
        }
    }
}