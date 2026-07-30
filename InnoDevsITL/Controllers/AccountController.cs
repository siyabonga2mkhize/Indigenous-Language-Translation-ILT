using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace InnoDevsITL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly InnoDbContext dbContext;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            SignInManager<Users> signInManager, 
            UserManager<Users> userManager, 
            InnoDbContext dbContext,
            ILogger<AccountController> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        logger.LogInformation($"User {user.Email} logged in successfully. Getting roles...");
                        
                        // Get all roles for this user
                        var roles = await userManager.GetRolesAsync(user);
                        logger.LogInformation($"User {user.Email} has roles: {string.Join(", ", roles)}");

                        // Redirect based on role
                        if (roles.Contains("Admin"))
                        {
                            logger.LogInformation($"Redirecting {user.Email} to Admin dashboard");
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (roles.Contains("Teacher"))
                        {
                            logger.LogInformation($"Redirecting {user.Email} to Teacher dashboard");
                            return RedirectToAction("Index", "Teacher");
                        }
                        else if (roles.Contains("Student"))
                        {
                            logger.LogInformation($"Redirecting {user.Email} to Student dashboard");
                            return RedirectToAction("Index", "Student");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Login succeeded but user {model.Email} not found!");
                    }

                    // Fallback if no role assigned
                    logger.LogWarning($"User {model.Email} logged in but has no role assigned. Redirecting to Home.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    logger.LogWarning($"Login failed for {model.Email}");
                    ModelState.AddModelError("", "Email or password is incorrect");
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            ViewBag.Faculties = new SelectList(dbContext.Faculties, "Id", "Name");
            ViewBag.Campuses = new SelectList(dbContext.Campuses, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PhysicalAddress = model.PhysicalAddress,
                    FacultyId = model.FacultyId,
                    CampusId = model.CampusId
                };

                var result = await userManager.CreateAsync(users, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(users, "Student");
                    logger.LogInformation($"New user {users.Email} registered as Student");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            ViewBag.Faculties = new SelectList(dbContext.Faculties, "Id", "Name");
            ViewBag.Campuses = new SelectList(dbContext.Campuses, "Id", "Name");
            return View(model);
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Something went wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }
            }
            return View(model);
        }

        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong. try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
