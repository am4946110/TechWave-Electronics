using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechWave_Electronics.Models;


namespace TechWave_Electronics.Controllers
{
    public class UserImageViewComponent : ViewComponent
    {
        private readonly UserManager<MyUser> _userManager;

        public UserImageViewComponent(UserManager<MyUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var imagePath = user?.ImagePath;
            return View(imagePath);
        }
    }
}
