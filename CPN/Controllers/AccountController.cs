using System.Linq;
using System.Web.Mvc;
using CPN.Models;

namespace CPN.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AppUser user)
        {
            if (db.AppUsers.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                // All new users are Student by default
                user.Role = "Student";
                db.AppUsers.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            var user = db.AppUsers.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                if (user.Role == role)
                {
                    // Store session
                    Session["Username"] = user.Username;
                    Session["Role"] = user.Role;

                    if (user.Role == "Admin")
                        return RedirectToAction("Dashboard", "Admin");
                    else
                        return RedirectToAction("Dashboard", "Student");
                }
                else
                {
                    ViewBag.Error = "Role does not match this username.";
                    return View();
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
