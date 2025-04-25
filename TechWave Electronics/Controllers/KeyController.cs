using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class KeyController : Controller
    {
        private readonly ICustomKeyManager _keyManager;

        public KeyController(ICustomKeyManager keyManager)
        {
            _keyManager = keyManager;
        }

        public IActionResult Update()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string newKey)
        {
            if (!string.IsNullOrEmpty(newKey))
            {
                _keyManager.SetKey(newKey);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Key cannot be empty.");
            return View();
        }
    }
}
