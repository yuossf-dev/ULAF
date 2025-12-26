using Microsoft.AspNetCore.Mvc;
using EntityFrameWork_Pro.Models;
using EntityFrameWork_Pro.DataBaseB;
using EntityFrameWork_Pro.Services;
using System.Linq;
using System.Text.RegularExpressions;

namespace EntityFrameWork_Pro.Controllers
{
    public class UserController : Controller
    {
        private readonly DBBridge _db;
        private readonly MicrosoftGraphService _graphService;
        private readonly EmailServiceResend _emailService;

        public UserController(DBBridge db, MicrosoftGraphService graphService, EmailServiceResend emailService)
        {
            _db = db;
            _graphService = graphService;
            _emailService = emailService;
        }

        // SignUp
        public IActionResult SignUp() => View();

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || user.UserName.Length < 4)
            {
                ViewBag.Error = "Username must be at least 4 characters";
                return View();
            }

            Regex passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d).{8,}$");
            if (!passwordRegex.IsMatch(user.Password))
            {
                ViewBag.Error = "Password must contain uppercase letter, number and be at least 8 characters";
                return View();
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ViewBag.Error = "Email is required";
                return View();
            }

            if (_db.Users.Any(u => u.UserName == user.UserName || u.Email == user.Email))
            {
                ViewBag.Error = "Username or Email already exists";
                return View();
            }

            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        // Register
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Validate Student ID
            if (string.IsNullOrWhiteSpace(user.StudentId))
            {
                ViewBag.Error = "الرقم الجامعي مطلوب";
                return View("SignUp");
            }

            // MUST validate with Microsoft Graph
            if (!_graphService.IsConfigured)
            {
                ViewBag.Error = "خدمة التحقق غير متاحة حالياً";
                return View("SignUp");
            }

            Console.WriteLine($"[DEBUG] Validating Student ID: {user.StudentId}");
            var studentInfo = await _graphService.GetStudentInfoAsync(user.StudentId);
            
            if (studentInfo == null)
            {
                Console.WriteLine($"[DEBUG] Student ID NOT FOUND: {user.StudentId}");
                ViewBag.Error = "الرقم الجامعي غير موجود في نظام الجامعة";
                return View("SignUp");
            }

            Console.WriteLine($"[DEBUG] Student FOUND: {studentInfo.Name}, Email: {studentInfo.Email}");
            
            // ✅ AUTO-FILL from university data (NOT from user input!)
            user.UserName = studentInfo.Name;
            user.Email = studentInfo.Email;
            user.Phone = ""; // No phone required

            // Validate password
            Regex passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d).{8,}$");
            if (!passwordRegex.IsMatch(user.Password))
            {
                ViewBag.Error = "كلمة المرور يجب أن تحتوي على حرف كبير ورقم ولا تقل عن 8 خانات";
                return View("SignUp");
            }

            // Check if already registered
            if (_db.Users.Any(u => u.StudentId == user.StudentId))
            {
                ViewBag.Error = "هذا الرقم الجامعي مسجل بالفعل";
                return View("SignUp");
            }

            // 📧 SEND VERIFICATION CODE
            string code = _emailService.GenerateVerificationCode();
            _emailService.StoreVerificationCode(user.StudentId, code);
            
            // Construct university email: studentId@zu.edu.jo
            string universityEmail = $"{user.StudentId}@zu.edu.jo";
            await _emailService.SendVerificationEmailAsync(universityEmail, code, user.UserName);

            Console.WriteLine($"📧 Verification code sent to: {universityEmail}");
            Console.WriteLine($"🔑 Code: {code}");

            // Store user data temporarily (for verification step)
            TempData["PendingUser"] = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            TempData["StudentEmail"] = universityEmail;
            TempData["StudentName"] = user.UserName;

            return RedirectToAction("VerifyEmail");
        }

        // Step 2: Verify Email
        public IActionResult VerifyEmail()
        {
            if (TempData["StudentEmail"] == null)
            {
                return RedirectToAction("Register");
            }

            ViewBag.Email = TempData["StudentEmail"];
            ViewBag.Name = TempData["StudentName"];
            TempData.Keep(); // Keep for POST
            return View();
        }

        [HttpPost]
        public IActionResult VerifyEmail(string verificationCode)
        {
            if (TempData["PendingUser"] == null)
            {
                ViewBag.Error = "انتهت صلاحية الجلسة. الرجاء المحاولة مرة أخرى";
                return RedirectToAction("Register");
            }

            var userJson = TempData["PendingUser"].ToString();
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userJson);

            if (string.IsNullOrWhiteSpace(verificationCode))
            {
                ViewBag.Error = "الرجاء إدخال رمز التحقق";
                ViewBag.Email = TempData["StudentEmail"];
                ViewBag.Name = TempData["StudentName"];
                TempData.Keep();
                return View();
            }

            // Verify the code
            if (!_emailService.VerifyCode(user.StudentId, verificationCode.Trim()))
            {
                ViewBag.Error = "رمز التحقق غير صحيح";
                ViewBag.Email = TempData["StudentEmail"];
                ViewBag.Name = TempData["StudentName"];
                TempData.Keep();
                return View();
            }

            // ✅ CODE VERIFIED! Create the account
            user.IsEmailVerified = true; // Mark as verified
            _db.Users.Add(user);
            _db.SaveChanges();

            Console.WriteLine($"✅ Account verified and created for: {user.UserName}");
            TempData["SuccessMessage"] = "تم تفعيل الحساب بنجاح! يمكنك الآن تسجيل الدخول";
            return RedirectToAction("Login");
        }

        // Login
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string studentId, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.StudentId == studentId && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "الرقم الجامعي أو كلمة المرور غير صحيحة";
                return View();
            }

            // 🔒 CHECK IF EMAIL IS VERIFIED
            if (!user.IsEmailVerified)
            {
                ViewBag.Error = "يجب تفعيل البريد الإلكتروني أولاً. تحقق من بريدك الجامعي";
                return View();
            }

            // Save user info in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("StudentId", user.StudentId);

            // Redirect to items page
            return RedirectToAction("AllItems", "Items");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
