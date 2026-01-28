using Microsoft.AspNetCore.Mvc;
using ClaimMonthlySystem.Services;
using ClaimMonthlySystem.Models;


namespace ClaimMonthlySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClaimService _claimService;

        public HomeController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            // If user is already logged in, redirect to appropriate dashboard
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                var role = HttpContext.Session.GetString("Role");
                if (role == "Lecturer")
                {
                    return RedirectToAction("Index", "Lecturer");
                }
                else if (role == "Manager")
                {
                    return RedirectToAction("Index", "Manager");
                }
            }
            
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Redirect if already logged in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToDashboard();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _claimService.Authenticate(username, password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Name", user.Name);

                TempData["Success"] = $"Welcome back, {user.Name}!";
                return RedirectToDashboard();
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Redirect if already registered in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToDashboard();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string confirmPassword)
        {
            // Basic validation
            if (user.Password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View(user);
            }

            if (_claimService.UsernameExists(user.Username))
            {
                ViewBag.Error = "Username already exists. Please choose a different username.";
                return View(user);
            }

            // Register the user - automatically as Lecturer
            if (_claimService.Register(user))                                                                                                                                                          
            {
                TempData["Success"] = "Registration successful! Please login with your credentials.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration failed. Please try again.";
            return View(user);
        }

        [HttpGet]
        public IActionResult ManagerLogin()
        {
            // Redirect if already logged in as manager
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")) && 
                HttpContext.Session.GetString("Role") == "Manager")
            {
                return RedirectToAction("Index", "Manager");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ManagerLogin(string username, string password)
        {
            var user = _claimService.Authenticate(username, password);
            if (user != null && user.Role == "Manager")
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Name", user.Name);

                TempData["Success"] = $"Welcome back, {user.Name}!";
                return RedirectToAction("Index", "Manager");
            }

            ViewBag.Error = "Invalid manager credentials";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index");
        }

        private IActionResult RedirectToDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Lecturer")
            {
                return RedirectToAction("Index", "Lecturer");
            }
            else if (role == "Manager")
            {
                return RedirectToAction("Index", "Manager");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
/*References
Anon., 2024. Simple Login Application using Sessions in ASP.NET MVC. [Online] 
Available at: https://www.c-sharpcorner.com/article/simple-login-application-using-Asp-Net-mvc/
Anon., 2025. Complete Login And Registration System In ASP.NET. [Online] 
Available at: https://www.c-sharpcorner.com/article/simple-login-and-registration-form-in-asp-net-mvc-using-ado-net/
Kralj, D., 2025. creating a login controller in MVC project. [Online] 
Available at: https://stackoverflow.com/questions/22099607/creating-a-login-controller-in-mvc-project

*/