using CPN.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace CPN.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // -------------------- DASHBOARD --------------------
        public ActionResult Dashboard()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // -------------------- MANAGE STUDENTS --------------------
        public ActionResult ManageStudents()
        {
            var students = db.AppUsers.Where(u => u.Role == "Student").ToList();
            return View(students);
        }


        // GET: Edit Student
        public ActionResult EditStudent(int id)
        {
            var student = db.AppUsers.Find(id);
            if (student == null || student.Role != "Student")
                return HttpNotFound();

            return View(student);
        }

        // POST: Edit Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(AppUser updatedStudent, string Password)
        {
            if (ModelState.IsValid)
            {
                var student = db.AppUsers.Find(updatedStudent.Id);
                if (student != null)
                {
                    student.Username = updatedStudent.Username;
                    student.Email = updatedStudent.Email;
                    student.Session = updatedStudent.Session;

                    // Update password only if provided
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        student.Password = Password; // Or hash it if needed
                    }

                    db.SaveChanges();
                    return RedirectToAction("ManageStudents");
                }
            }

            return View(updatedStudent);
        }



        public ActionResult DeleteStudent(int id)
        {
            var student = db.AppUsers.Find(id);
            if (student != null && student.Role == "Student")
            {
                db.AppUsers.Remove(student);
                db.SaveChanges();
            }
            return RedirectToAction("ManageStudents");
        }

        // -------------------- MANAGE JOBS --------------------
        public ActionResult ManageJobs(string department)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (department == "CS")
            {
                ViewBag.Department = "CS";
                return View("ManageJobs", db.CSJobs.ToList());
            }
            else if (department == "CT")
            {
                ViewBag.Department = "CT";
                return View("ManageJobs", db.CTJobs.ToList());
            }
            return HttpNotFound();
        }

        // -------------------- MANAGE QUESTIONS --------------------
        public ActionResult ManageQuestions(string department, int jobId)
        {
            string jobTitle = "";

            if (department == "CS")
            {
                var job = db.CSJobs.Find(jobId);
                if (job != null)
                    jobTitle = job.Title;
            }
            else if (department == "CT")
            {
                var job = db.CTJobs.Find(jobId);
                if (job != null)
                    jobTitle = job.Title;
            }

            // Store job title in session
            Session["JobTitle"] = jobTitle;

            // Load questions based on department and jobId
            IEnumerable<dynamic> questions = null;

            if (department == "CS")
            {
                questions = db.CSJobQuestions.Where(q => q.JobId == jobId).ToList();
            }
            else if (department == "CT")
            {
                questions = db.CTJobQuestions.Where(q => q.JobId == jobId).ToList();
            }

            ViewBag.Department = department;
            ViewBag.JobId = jobId;

            return View(questions);
        }


        // -------------------- ADD JOB (GET) --------------------
        public ActionResult AddJob(string department)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (department == "CS")
            {
                ViewBag.Department = "CS";
                return View("AddCSJob", new CSJob());
            }
            else if (department == "CT")
            {
                ViewBag.Department = "CT";
                return View("AddCTJob", new CTJob());
            }

            return HttpNotFound();
        }

        // -------------------- ADD JOB (POST) --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCSJob(CSJob job)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.CSJobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("ManageJobs", new { department = "CS" });
            }

            ViewBag.Department = "CS";
            return View("AddCSJob", job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCTJob(CTJob job)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.CTJobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("ManageJobs", new { department = "CT" });
            }

            ViewBag.Department = "CT";
            return View("AddCTJob", job);
        }

        // -------------------- ADD QUESTION (GET) --------------------
        public ActionResult AddQuestion(string department, int jobId)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (department == "CS")
            {
                var job = db.CSJobs.Find(jobId);
                if (job == null) return HttpNotFound();
                ViewBag.JobTitle = job.Title;
                ViewBag.Department = "CS";
                return View("AddCSQuestion", new CSJobQuestion { JobId = jobId });
            }
            else if (department == "CT")
            {
                var job = db.CTJobs.Find(jobId);
                if (job == null) return HttpNotFound();
                ViewBag.JobTitle = job.Title;
                ViewBag.Department = "CT";
                return View("AddCTQuestion", new CTJobQuestion { JobId = jobId });
            }

            return HttpNotFound();
        }

        // -------------------- ADD QUESTION (POST) --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCSQuestion(CSJobQuestion model)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.CSJobQuestions.Add(model);
                db.SaveChanges();
                return RedirectToAction("ManageQuestions", new { department = "CS", jobId = model.JobId });
            }

            var job = db.CSJobs.Find(model.JobId);
            ViewBag.JobTitle = job?.Title;
            ViewBag.Department = "CS";
            return View("AddCSQuestion", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCTQuestion(CTJobQuestion model)
        {
            if (Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.CTJobQuestions.Add(model);
                db.SaveChanges();
                return RedirectToAction("ManageQuestions", new { department = "CT", jobId = model.JobId });
            }

            var job = db.CTJobs.Find(model.JobId);
            ViewBag.JobTitle = job?.Title;
            ViewBag.Department = "CT";
            return View("AddCTQuestion", model);
        }

        // -------------------- DISPOSE --------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // DELETE: CS Job
        public ActionResult DeleteCSJob(int id)
        {
            var job = db.CSJobs.Find(id);
            if (job != null)
            {
                // Related Questions ကိုလည်း ဖျက်မယ်
                var relatedQuestions = db.CSJobQuestions.Where(q => q.JobId == id).ToList();
                foreach (var q in relatedQuestions)
                {
                    db.CSJobQuestions.Remove(q);
                }

                db.CSJobs.Remove(job);
                db.SaveChanges();
            }
            return RedirectToAction("ManageJobs", new { department = "CS" });
        }

        // DELETE: CT Job
        public ActionResult DeleteCTJob(int id)
        {
            var job = db.CTJobs.Find(id);
            if (job != null)
            {
                var relatedQuestions = db.CTJobQuestions.Where(q => q.JobId == id).ToList();
                foreach (var q in relatedQuestions)
                {
                    db.CTJobQuestions.Remove(q);
                }

                db.CTJobs.Remove(job);
                db.SaveChanges();
            }
            return RedirectToAction("ManageJobs", new { department = "CT" });
        }
        // DELETE: CS Question
        public ActionResult DeleteCSQuestion(int id)
        {
            var question = db.CSJobQuestions.Find(id);
            if (question != null)
            {
                int jobId = question.JobId;
                db.CSJobQuestions.Remove(question);
                db.SaveChanges();
                return RedirectToAction("ManageQuestions", new { department = "CS", jobId = jobId });
            }
            return HttpNotFound();
        }

        // DELETE: CT Question
        public ActionResult DeleteCTQuestion(int id)
        {
            var question = db.CTJobQuestions.Find(id);
            if (question != null)
            {
                int jobId = question.JobId;
                db.CTJobQuestions.Remove(question);
                db.SaveChanges();
                return RedirectToAction("ManageQuestions", new { department = "CT", jobId = jobId });
            }
            return HttpNotFound();
        }


        public ActionResult AddPost()
        {
           

            return View();
        }

        // POST: Add Post with Video Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPost(Post post, HttpPostedFileBase videoFile)
        {
            

            if (videoFile != null && videoFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(videoFile.FileName);
                string videosFolder = Server.MapPath("~/Videos/");

                if (!Directory.Exists(videosFolder))
                {
                    Directory.CreateDirectory(videosFolder);
                }

                string path = Path.Combine(videosFolder, fileName);
                videoFile.SaveAs(path);

                post.VideoPath = "/Videos/" + fileName;
            }

            post.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("ManagePosts");
            }

            return View(post);
        }

        // GET: Manage Posts
        public ActionResult ManagePosts()
        {
            

            var posts = db.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var post = db.Posts.Find(id);
            if (post != null)
            {
                // Delete video file if exists
                if (!string.IsNullOrEmpty(post.VideoPath))
                {
                    var fullPath = Server.MapPath(post.VideoPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                db.Posts.Remove(post);
                db.SaveChanges();
            }

            return RedirectToAction("ManagePosts"); // Update if your listing action is different
        }


        // GET: Admin/ViewFeedback
        public ActionResult ViewFeedback()
        {
            // Fetch all feedback, latest first
            var feedbackList = db.Feedbacks.OrderByDescending(f => f.CreatedAt).ToList();
            return View(feedbackList);
        }

    }
}
