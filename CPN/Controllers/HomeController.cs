using CPN.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CPN.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult AdminHome()
        {
            if (Session["Role"]?.ToString() == "Admin")
                return View();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult StudentHome()
        {
            if (Session["Role"]?.ToString() == "Student")
                return View();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Index()
        {
            var csJobs = db.CSJobs.Select(j => new JobViewModel
            {
                Id = j.Id,
                Title = j.Title,
                Department = "CS"
            });

            var ctJobs = db.CTJobs.Select(j => new JobViewModel
            {
                Id = j.Id,
                Title = j.Title,
                Department = "CT"
            });

            var allJobs = csJobs.Concat(ctJobs).ToList();

            return View(allJobs);
        }

        public ActionResult About()
        {
            return View();
        }

        // Contact GET
        public ActionResult Contact()
        {
            return View();
        }


    }
}
