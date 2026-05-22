using Lap02.Models;
using Lap02.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lap02.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AuthController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserAccount account)
        {
            var user = _accountRepository.Login(account.Username!, account.Password!);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username!);
            HttpContext.Session.SetString("Role", user.Role!);

            return RedirectToAction("Index", "Product");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
