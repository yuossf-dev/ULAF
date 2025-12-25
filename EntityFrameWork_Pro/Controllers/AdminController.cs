using Microsoft.AspNetCore.Mvc;
using EntityFrameWork_Pro.Services;
using EntityFrameWork_Pro.Interfaces;
using EntityFrameWork_Pro.Models;

namespace EntityFrameWork_Pro.Controllers
{
    public class AdminController : Controller
    {
        private const string AdminPassword = "admin123"; // Change this!
        private readonly IRepository _repository;

        public AdminController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            // Check if admin is logged in
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.CurrentMode = DatabaseMode.GetCurrentMode();
            ViewBag.IsOnline = DatabaseMode.IsOnline;
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == AdminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult SwitchMode(bool online)
        {
            // Check if admin is logged in
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            DatabaseMode.IsOnline = online;
            TempData["Success"] = $"Switched to {(online ? "Online" : "Offline")} mode successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageUsers()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var users = await _repository.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> ManageItems()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var items = await _repository.GetAllItemsAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string studentId)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var user = await _repository.GetUserByStudentIdAsync(studentId);
            if (user != null)
            {
                await _repository.DeleteUserAsync(user);
                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "User not found!";
            }

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var item = await _repository.GetItemByIdAsync(id);
            if (item != null)
            {
                await _repository.DeleteItemAsync(item);
                TempData["Success"] = "Item deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Item not found!";
            }

            return RedirectToAction("ManageItems");
        }
    }
}
