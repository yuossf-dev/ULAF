using Microsoft.AspNetCore.Mvc;
using EntityFrameWork_Pro.Models;
using EntityFrameWork_Pro.DataBaseB;
using EntityFrameWork_Pro.Interfaces;

namespace EntityFrameWork_Pro.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemRepository _itemRepo;
        private readonly IUserRepository _userRepo;
        private readonly IWebHostEnvironment _env;

        public ItemsController(IItemRepository itemRepo, IUserRepository userRepo, IWebHostEnvironment env)
        {
            _itemRepo = itemRepo;
            _userRepo = userRepo;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            return RedirectToAction("AllItems");
        }

        [HttpGet]
        public IActionResult Create(string status)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.Status = status;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Item model, List<IFormFile> files)
        {
            // ✅ Get logged-in user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            
            model.UserId = userId.Value;
            
            // ✅ Load user name from repository for Firebase
            try
            {
                var userName = HttpContext.Session.GetString("UserName");
                if (!string.IsNullOrEmpty(userName))
                {
                    model.PostedBy = new User { UserName = userName };
                    Console.WriteLine($"[ITEM-CREATE] Setting poster name: {userName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ITEM-CREATE] ⚠️ Could not load user name: {ex.Message}");
            }
            
            model.MediaPaths = new List<string>();

            string uploadFolder = Path.Combine(_env.WebRootPath, "Uploads");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            foreach (var file in files)
            {
                string filePath = Path.Combine(uploadFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                model.MediaPaths.Add("/Uploads/" + file.FileName);
            }

            await _itemRepo.AddItemAsync(model);

            // Find matches
            var matches = FindMatches(model);
            ViewBag.Matches = matches;

            return View("ShowData", model);
        }

        private List<(Item item, int score)> FindMatches(Item newItem)
        {
            var oppositeType = newItem.Status == "Lost" ? "Found" : "Lost";
            var allItems = _itemRepo.GetAllItems().Where(i => i.Status == oppositeType).ToList();
            var matches = new List<(Item item, int score)>();

            foreach (var item in allItems)
            {
                int score = 0;

                // Same category - 40% (REQUIRED)
                if (string.IsNullOrEmpty(item.Category) || string.IsNullOrEmpty(newItem.Category) ||
                    !item.Category.Equals(newItem.Category, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip if category doesn't match
                }
                score += 40;

                // Same or nearby location - 30%
                if (!string.IsNullOrEmpty(item.Location) && !string.IsNullOrEmpty(newItem.Location))
                {
                    var itemLoc = item.Location.ToLower().Trim();
                    var newLoc = newItem.Location.ToLower().Trim();
                    
                    if (itemLoc == newLoc)
                        score += 30; // Exact match
                    else if (itemLoc.Contains(newLoc) || newLoc.Contains(itemLoc))
                        score += 20; // Partial match
                }

                // Date within 7 days - 15%
                var dateDiff = Math.Abs((item.Date - newItem.Date).TotalDays);
                if (dateDiff <= 3)
                    score += 15;
                else if (dateDiff <= 7)
                    score += 10;

                // Name similarity - 15%
                if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(newItem.Name))
                {
                    var itemName = item.Name.ToLower().Trim();
                    var newName = newItem.Name.ToLower().Trim();
                    
                    // Check word overlap
                    var itemWords = itemName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var newWords = newName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var commonWords = itemWords.Intersect(newWords).Count();
                    
                    if (itemName == newName)
                        score += 15; // Exact match
                    else if (commonWords > 0)
                        score += (int)(15 * ((double)commonWords / Math.Max(itemWords.Length, newWords.Length)));
                }

                // Description similarity (if provided) - Bonus points
                if (!string.IsNullOrEmpty(item.Description) && !string.IsNullOrEmpty(newItem.Description))
                {
                    var itemDesc = item.Description.ToLower();
                    var newDesc = newItem.Description.ToLower();
                    var descWords = newDesc.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var matchCount = descWords.Count(w => w.Length > 3 && itemDesc.Contains(w));
                    if (matchCount > 0)
                        score += Math.Min(10, matchCount * 2);
                }

                // Only show matches with minimum 60% score
                if (score >= 60)
                {
                    matches.Add((item: item, score: score));
                }
            }

            return matches.OrderByDescending(m => m.score).Take(5).ToList();
        }

        public async Task<IActionResult> LostItems()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            var lostItems = await _itemRepo.GetLostItemsAsync();
            return View(lostItems);
        }

        public async Task<IActionResult> FoundItems()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            var foundItems = await _itemRepo.GetFoundItemsAsync();
            return View(foundItems);
        }

        public async Task<IActionResult> AllItems()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            var items = await _itemRepo.GetAllItemsAsync();
            return View(items);
        }
    }
}
