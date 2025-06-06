using KapdaStore.Data;
using KapdaStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace KapdaStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Register() => View();
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();
                string hashedPassword = passwordHasher.HashPassword(user, user.PasswordHash);
                user.PasswordHash = hashedPassword;

                _db.Users.Add(user);
                _db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }

            [HttpPost]
            public IActionResult Login(string email, string password)
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                    if (result == PasswordVerificationResult.Success)
                    {
                        // ✅ Login successful
                        HttpContext.Session.SetString("UserEmail", user.Email);
                    return RedirectToAction("Index", "Prakash");
                }
            }

                // ❌ Login failed
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
        

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
