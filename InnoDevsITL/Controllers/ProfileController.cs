// Controllers/ProfileController.cs
using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly InnoDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            InnoDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var model = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PhysicalAddress = user.PhysicalAddress,
                DateOfBirth = user.DateOfBirth,
                FacultyId = user.FacultyId,
                CampusId = user.CampusId,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            ViewBag.Roles = roles;
            ViewBag.Faculties = await _context.Faculties.ToListAsync();
            ViewBag.Campuses = await _context.Campuses.ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.PhysicalAddress = model.PhysicalAddress;
                user.FacultyId = model.FacultyId;
                user.CampusId = model.CampusId;

                // Handle profile picture upload
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/profile");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{user.Id}_{DateTime.Now.Ticks}_{Path.GetFileName(model.ProfilePicture.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePictureUrl = $"/uploads/profile/{uniqueFileName}";
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Faculties = await _context.Faculties.ToListAsync();
            ViewBag.Campuses = await _context.Campuses.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        TempData["Success"] = "Password changed successfully!";
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}