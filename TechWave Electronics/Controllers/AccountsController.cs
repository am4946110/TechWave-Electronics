using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using TechWave_Electronics.Models;
using System.IO;
using System.IO.MemoryMappedFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32.SafeHandles;
using System;
using static TechWave_Electronics.Models.ViewModels;
using Path = System.IO.Path;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using IdentityUser = TechWave_Electronics.Models.ApplicationDbContext;


namespace TechWave_Electronics.Controllers
{
    
    [ActivityLog]
    public class AccountsController : Controller
    {
        private readonly UserManager<MyUser> userManager;
        private readonly RoleManager<MyRole> roleManager;
        private readonly SignInManager<MyUser> signinManager;
        private readonly IWebHostEnvironment _environment;
        private readonly SafeFileHandle imagePath;
        private readonly ILogger<AccountsController> _logger;
        private readonly ApplicationDbContext dentityUser;
        private readonly ICustomKeyManager _keyManager;
        private MyUser user;
        public AccountsController(ILogger<AccountsController> logger, ApplicationDbContext dentity, ICustomKeyManager keyManager,  UserManager<MyUser> um, RoleManager<MyRole> rm, SignInManager<MyUser> sm)
        {
            _logger = logger;
            userManager = um;
            roleManager = rm;
            signinManager = sm;
            dentityUser = dentity;
            _keyManager = keyManager;
        }



        public string UserName { get; private set; }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Invalid registration data.");
                return View(model);
            }

            MyUser user = new MyUser { UserName = model.Email, Email = model.Email };
            var identityResult = await userManager.CreateAsync(user, model.Password);

            if (identityResult.Succeeded)
            {
                MyRole? r = await roleManager.FindByNameAsync("client");
                if (r == null)
                {
                    r = new MyRole("client");
                    var roleResult = await roleManager.CreateAsync(r);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to create role.");
                        return View(model);
                    }
                }

                if (!string.IsNullOrEmpty(r.Name))
                {
                    await userManager.AddToRoleAsync(user, r.Name);
                }
                else
                {
                    ModelState.AddModelError("", "Role name is invalid.");
                    return View(model);
                }

                return RedirectToAction("Login");
            }

            foreach (var item in identityResult.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return View(model);
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

           
            if (await userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
                return View(model);
            }

            var result = await signinManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
               
                await userManager.ResetAccessFailedCountAsync(user);
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }
            else
            {
                
                await userManager.AccessFailedAsync(user);
                var accessFailedCount = await userManager.GetAccessFailedCountAsync(user);

                if (accessFailedCount >= 3)
                {
                    
                    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddHours(3));
                    ModelState.AddModelError(string.Empty, "Your account has been locked due to multiple failed login attempts. Please try again after 3 hours.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

                // التحقق مما إذا كان الحساب قد تم قفله سابقًا وتمت محاولات فاشلة بعد فك القفل
                if (user.LockoutEnd != null && user.LockoutEnd <= DateTimeOffset.Now)
                {
                    // حذف الحساب بعد محاولات فاشلة متكررة
                    await userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Your account has been deleted due to multiple failed login attempts.");
                }

                return View(model);
            }
        }


        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Removing Session
                HttpContext.Session.Clear();

                // Removing Cookies
                CookieOptions option = new CookieOptions();
                if (Request.Cookies[".AdventureWorks.Session"] != null)
                {
                    option.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Append(".AdventureWorks.Session", "", option);
                }

                if (Request.Cookies[".AspNetCore.Antiforgery.hcNAj-zuhGg"] != null)
                {
                    option.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Append(".AspNetCore.Antiforgery.hcNAj-zuhGg", "", option);
                }

                await signinManager.SignOutAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        
        public IActionResult AccessDenided()
        {
            return View();
        }

        [Authorize(Roles = "Administrators,It,It Management")]
        public IActionResult Index()
        {
            var users = userManager.Users
                .Select(user => new MyUser
                {
                    usersId = _keyManager.Protect(user.Id.ToString()),
                  UserName = user.UserName,
                    Email = user.Email
                })
                .ToList();

            return View(users);
        }

        [Authorize(Roles = "Administrators,It,It Management")]
        [HttpGet]
        public async Task<IActionResult> Assign(string id)
        {
            try
            {
                // التحقق من أن id غير فارغ أو null
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("Invalid ID format.");
                }

                // فك تشفير الـ id
                var userId = _keyManager.Unprotect(id);
                Console.WriteLine($"Unprotected ID: {userId}");

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(); // إذا لم يتم العثور على المستخدم، ارجع 404
                }

                var currentRoles = await userManager.GetRolesAsync(user); // الحصول على الأدوار الحالية للمستخدم
                var selectedRole = currentRoles.FirstOrDefault(); // اختيار الدور الأول للمستخدم

                string selectedRoleId = null;
                if (!string.IsNullOrEmpty(selectedRole))
                {
                    var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == selectedRole); // الحصول على الدور بناءً على الاسم
                    selectedRoleId = role?.Id;
                }

                var model = new UserRoleModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoleId = selectedRoleId
                };

                ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name", model.RoleId); // عرض قائمة الأدوار

                return View(model); // إرجاع العرض مع النموذج
            }
            catch (CryptographicException ex)
            {
                return BadRequest("Invalid ID format.");
            }
        }



        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleModel model)
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.EncryptedId))
            {
                ModelState.AddModelError("", "Invalid ID or Role selection.");
                ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                return View(model);
            }

            try
            {
                // Unprotect the user ID from the model
                var userId = _keyManager.Unprotect(model.UserId);

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                // Unprotect the encrypted role ID
                var roleId = _keyManager.Unprotect(model.EncryptedId);
                var newRole = await roleManager.FindByIdAsync(roleId);
                if (newRole == null || string.IsNullOrEmpty(newRole.Name))
                {
                    ModelState.AddModelError("", "Invalid role selected.");
                    ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                    return View(model);
                }

                var currentRoles = await userManager.GetRolesAsync(user) ?? new List<string>();

                if (currentRoles.Contains(newRole.Name))
                {
                    ModelState.AddModelError("", $"User {user.UserName} is already assigned to the '{newRole.Name}' role.");
                    ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                    return View(model);
                }

                foreach (var role in currentRoles)
                {
                    var removeResult = await userManager.RemoveFromRoleAsync(user, role);
                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError("", $"Error removing user from role: {role}");
                        ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                        return View(model);
                    }
                }

                var addResult = await userManager.AddToRoleAsync(user, newRole.Name);
                if (addResult.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    await signinManager.SignOutAsync();
                    return RedirectToAction("Index");
                }

                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                return View(model);
            }
            catch (ArgumentNullException ex)
            {
                // Handle ArgumentNullException
                ModelState.AddModelError("", $"Error: {ex.Message}");
                ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                return View(model);
            }
            catch (Exception ex)
            {
                // Catch any other exceptions and display the error
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                ViewBag.RoleId = new SelectList(roleManager.Roles, "Id", "Name");
                return View(model);
            }
        }

        [Authorize]
        public IActionResult Profile()
        {
            var user = dentityUser.MyUsers.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                var encryptedId = _keyManager.Protect(user.Id.ToString());
                ViewData["EncryptedId"] = encryptedId;
                ViewBag.User = user;
                return View(user);
            }
            catch (CryptographicException ex)
            {
                // Log the exception or handle it accordingly
                return BadRequest("Error while protecting user ID.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string email, string id)
        {
            try
            {
                // فك تشفير المعرف
                string userId = _keyManager.Unprotect(id);

                // التحقق من صحة المعلمات
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId))
                {
                    return NotFound("البيانات المدخلة غير صالحة.");
                }

                // البحث عن المستخدم باستخدام المعرف
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("المستخدم غير موجود.");
                }

                // تجهيز البيانات لعرضها في الـ View
                var viewModel = new MyUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Country = user.Country,
                    PhoneNumber = user.PhoneNumber,
                    BirthDate = user.BirthDate,
                    ImageFile = user.ImageFile
                };

                // إرسال المعرف المشفر إلى ViewData لاستخدامه في العرض
                ViewData["EncryptedId"] = id;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // تسجيل الاستثناء و إرجاع استجابة مناسبة
                _logger.LogError(ex, "خطأ في تحميل بيانات المستخدم.");
                return BadRequest("حدث خطأ أثناء معالجة الطلب.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MyUser viewModel, string id)
        {
            if (viewModel == null)
            {
                return BadRequest("Invalid model.");
            }

            if (string.IsNullOrEmpty(viewModel.Id))
            {
                return NotFound();
            }

            string decryptedId;
            try
            {
                decryptedId = _keyManager.Unprotect(viewModel.Id.ToString());
            }
            catch (Exception)
            {
                return BadRequest("Invalid request.");
            }

            if (dentityUser?.MyUsers == null)
            {
                return StatusCode(500, "Users data is not available.");
            }

            try
            {
                var andie = await dentityUser.MyUsers.FindAsync(decryptedId);
                if (andie == null)
                {
                    return NotFound();
                }

                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await viewModel.ImageFile.CopyToAsync(memoryStream);
                        andie.ImagePath = memoryStream.ToArray();
                    }
                }

                andie.UserName = viewModel.UserName;
                andie.Email = viewModel.Email;
                andie.PhoneNumber = viewModel.PhoneNumber;
                andie.Country = viewModel.Country; // Ensure this is correct
                andie.BirthDate = viewModel.BirthDate;

                dentityUser.MyUsers.Update(andie);
                await dentityUser.SaveChangesAsync();
                return RedirectToAction(nameof(Profile));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AudienceExists(viewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "The changes could not be saved. Try again, and if the problem persists, contact your system administrator.");
            }

            ViewData["EncryptedId"] = viewModel.Id;
            return View(viewModel);
        }



        private bool AudienceExists(string encryptedId)
        {
            string decryptedId;
            try
            {
                decryptedId = _keyManager.Unprotect(encryptedId);
            }
            catch
            {
                return false;
            }
            return dentityUser.MyUsers.Any(u => u.Id == decryptedId);
        }




        [Authorize(Roles = "Administrators,It Management")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            string userId;
            try
            {
                userId = _keyManager.Unprotect(id);
            }
            catch
            {
                return BadRequest("معرف المستخدم غير صالح.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // بإمكانك عرض ViewModel مختصر بدل تمرير الكائن كامل
            var viewModel = new MyUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Administrators,It Management")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "تم حذف المستخدم بنجاح.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // إعادة عرض بيانات المستخدم في حالة الفشل
            var viewModel = new MyUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return View("Delete", viewModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditPassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPassword(EditPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userId;
            try
            {
                userId = _keyManager.Unprotect(model.EncryptedId);
            }
            catch
            {
                ModelState.AddModelError("", "رابط غير صالح.");
                return View(model);
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                TempData["SuccessMessage"] = "تم تغيير كلمة المرور بنجاح.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}