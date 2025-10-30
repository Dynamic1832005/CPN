using CPN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CPN.Controllers
{
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            if (Session["Role"]?.ToString() != "Student")
                return RedirectToAction("Login", "Account");

            // Load CS jobs
            var csJobs = db.CSJobs.Select(j => new JobViewModel
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Department = "Computer Science",
                VideoUrl = null // or j.VideoUrl if exists
            }).ToList();

            // Load CT jobs
            var ctJobs = db.CTJobs.Select(j => new JobViewModel
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Department = "Computer Technology",
                VideoUrl = null // or j.VideoUrl if exists
            }).ToList();

            // Combine both lists
            var allJobs = csJobs.Concat(ctJobs).ToList();

            return View(allJobs);
        }


        // ✅ GET: Student/CSquiz
        public ActionResult CSquiz()
        {
            if (Session["Role"]?.ToString() != "Student")
                return RedirectToAction("Login", "Account");

            var jobTitles = db.CSJobs.ToList();
            var allQuestions = new List<CSJobQuestion>();

            foreach (var job in jobTitles)
            {
                var questions = db.CSJobQuestions
                                  .Where(q => q.JobId == job.Id)
                                  .OrderBy(q => Guid.NewGuid()) // Random
                                  .Take(10)
                                  .ToList();
                allQuestions.AddRange(questions);
            }

            return View("CSquiz", allQuestions);
        }

        // ✅ POST: Student/SubmitCSquiz
        [HttpPost]
        public ActionResult SubmitCSQuiz(FormCollection form)
        {
            var usernameObj = Session["Username"];
            if (usernameObj == null) return RedirectToAction("Login", "Account");
            string username = usernameObj.ToString();

            var user = db.AppUsers.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Account");

            var results = new Dictionary<string, int>();

            foreach (var job in db.CSJobs.ToList())
            {
                int score = 0;
                var questions = db.CSJobQuestions.Where(q => q.JobId == job.Id).ToList();
                foreach (var q in questions)
                {
                    string selected = form["q_" + q.Id];
                    if (selected == q.CorrectAnswer) score++;
                }

                results[job.Title] = score;

                db.QuizResults.Add(new QuizResult
                {
                    UserId = user.Id,
                    Department = "CS",
                    JobTitle = job.Title,
                    Score = score,
                    AttemptDate = DateTime.Now
                });
            }

            db.SaveChanges();
            ViewBag.Results = results;
            return View("CSResult");
        }


        // ✅ GET: Student/CTquiz
        public ActionResult CTquiz()
        {
            if (Session["Role"]?.ToString() != "Student")
                return RedirectToAction("Login", "Account");

            var jobTitles = db.CTJobs.ToList();
            var allQuestions = new List<CTJobQuestion>();

            foreach (var job in jobTitles)
            {
                var questions = db.CTJobQuestions
                                  .Where(q => q.JobId == job.Id)
                                  .OrderBy(q => Guid.NewGuid()) // Random
                                  .Take(10)
                                  .ToList();
                allQuestions.AddRange(questions);
            }

            return View("CTquiz", allQuestions);
        }

        // ✅ POST: Student/SubmitCTquiz
        [HttpPost]
        public ActionResult SubmitCTQuiz(FormCollection form)
        {
            var usernameObj = Session["Username"];
            if (usernameObj == null) return RedirectToAction("Login", "Account");
            string username = usernameObj.ToString();

            var user = db.AppUsers.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Account");

            var results = new Dictionary<string, int>();

            foreach (var job in db.CTJobs.ToList())
            {
                int score = 0;
                var questions = db.CTJobQuestions.Where(q => q.JobId == job.Id).ToList();
                foreach (var q in questions)
                {
                    string selected = form["q_" + q.Id];
                    if (selected == q.CorrectAnswer) score++;
                }

                results[job.Title] = score;

                db.QuizResults.Add(new QuizResult
                {
                    UserId = user.Id,
                    Department = "CT",
                    JobTitle = job.Title,
                    Score = score,
                    AttemptDate = DateTime.Now
                });
            }

            db.SaveChanges();
            ViewBag.Results = results;
            return View("CTResult");
        }


        // ✅ GET: Student/CTResult
        public ActionResult CTResult()
        {
            if (Session["Role"]?.ToString() != "Student")
                return RedirectToAction("Login", "Account");

            var results = TempData["CTResults"] as Dictionary<string, int>;
            ViewBag.Results = results;

            return View("CTResult");
        }


        public ActionResult Quiz()
        {
            return View();
        }

        public ActionResult JobDetails(string title)
        {
            if (string.IsNullOrEmpty(title))
                return RedirectToAction("Dashboard");

            // Search in CSJobs
            var csJob = db.CSJobs
                .Where(j => j.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                .Select(j => new JobViewModel
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Department = "Computer Science",
                    VideoUrl = null // or j.VideoUrl if you have it
                })
                .FirstOrDefault();

            if (csJob != null)
                return View(csJob);

            // Search in CTJobs if not found in CSJobs
            var ctJob = db.CTJobs
                .Where(j => j.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                .Select(j => new JobViewModel
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Department = "Computer Technology",
                    VideoUrl = null // or j.VideoUrl if you have it
                })
                .FirstOrDefault();

            if (ctJob != null)
                return View(ctJob);

            // If not found in both, return 404
            return HttpNotFound();
        }


        public ActionResult Posts()
        {
            var posts = db.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            return View(posts);
        }

        public ActionResult PostDetails(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var post = db.Posts.Find(id);
            if (post == null)
                return HttpNotFound();

            return View(post);
        }



        // GET: Feedback
        [HttpGet]
        public ActionResult Feedback()
        {
            // Get the logged-in student
            string username = Session["Username"]?.ToString(); // adjust based on your login session
            var student = db.AppUsers.FirstOrDefault(u => u.Username == username);

            if (student == null)
                return RedirectToAction("Login", "Account");

            var model = new Feedback
            {
                Name = student.Username,
                Email = student.Email
            };

            return View(model); // returns Feedback.cshtml
        }

        // POST: Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(Feedback model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                db.Feedbacks.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Thank you for your feedback!";
                return RedirectToAction("Feedback");
            }

            return View(model);
        }

        // ---------------- Quiz History ----------------
        public ActionResult QuizHistory()
        {
            var usernameObj = Session["Username"];
            if (usernameObj == null) return RedirectToAction("Login", "Account");
            string username = usernameObj.ToString();

            var user = db.AppUsers.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Account");

            var history = db.QuizResults
                            .Where(q => q.UserId == user.Id)
                            .OrderByDescending(q => q.AttemptDate)
                            .ToList();

            return View(history); // strongly-typed IEnumerable<QuizResult>
        }

    }
}
